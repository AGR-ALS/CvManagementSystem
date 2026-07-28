import EditProfileForm from "../Components/ProfileForm/EditProfileForm";
import ViewProfileForm from "../Components/ProfileForm/ViewProfileForm";
import React, {useState} from "react";
import {UserProfile} from "../models/UserProfile";
import {useNavigate, useSearchParams} from "react-router";
import {useAutoSave} from "../Hooks/useAutoSave";
import ProjectEditModal from "../Components/ProjectEditModal/ProjectEditModal";
import {ProjectContext} from "../Contexts/ProjectContext";
import ProfileAttributeEditModal from "../Components/ProfileAttributeEditModal/ProfileAttributeEditModal";
import AttributeValueModal from "../Components/ProfileAttributeModal/ProfileAttributeModal";
import {AttributeContext} from "../Contexts/AttributeContext";
import {isAdmin} from "../utils/roles";
import {Spin} from "antd";
import {useProfilePage} from "../Hooks/useProfilePage";
import {useProfileProjects} from "../Hooks/useProfileProjects";
import {useProfileAttributes} from "../Hooks/useProfileAttributes";

export default function ProfilePage() {
    const [changedUser, setChangedUser] = useState<UserProfile | null>(null);

    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const page = useProfilePage(searchParams, navigate);
    const projects = useProfileProjects(page.user.id, (updatedProjects) => page.setUser(current => ({
        ...current,
        projects: updatedProjects
    })));
    const attributes = useProfileAttributes(page.user.id, page.user.attributeValues, (attributeValues) => page.setUser(current => ({
        ...current,
        attributeValues
    })), page.isViewMode);

    const handleOpenCv = (positionId: string) => {
        navigate(`/cv?userId=${encodeURIComponent(page.user.id)}&positionId=${encodeURIComponent(positionId)}&mode=view`);
    }

    const handleEdit = () => {
        const userIdParam = searchParams.get("userId");
        if (userIdParam) {
            navigate(`/profile?userId=${encodeURIComponent(userIdParam)}&mode=edit`);
        } else {
            navigate("/profile?mode=edit");
        }
    }

    const canEditProfile = isAdmin(page.currentUserRole) || (!!page.currentUserId && page.currentUserId === page.user.id);
    const canEditRole = isAdmin(page.currentUserRole);

    useAutoSave(
        page.isViewMode ? null : changedUser,
        page.handleSaveProfile,
        10000
    );

    if (page.loading) {
        return <div className="position-fixed top-50 start-50 translate-middle"><Spin size="large"/></div>;
    }

    return (
        <div>
            {!page.isViewMode && (
                <ProjectEditModal
                    open={projects.isProjectModalOpen}
                    project={projects.currentProject}
                    onCancel={projects.onCancel}
                    onSave={projects.onSave}
                    onDelete={projects.onDelete}
                    isCreating={projects.isProjectModalCreating}
                    technologyOptions={page.technologyOptions}
                    onSearchTechnologies={page.handleSearchTechnologies}
                />
            )}
            <ProjectContext value={{
                projects: page.user.projects,
                handleOpenCreation: projects.onOpenCreation,
                handleOpenProject: projects.onOpenProject,
                handleCancel: projects.onCancel,
                handleDelete: projects.onDelete,
                readOnly: page.isViewMode,
            }}>
                <AttributeContext value={{
                    data: page.user.attributeValues,
                    handleOpen: attributes.onOpen,
                    handleAdd: attributes.onAdd,
                    handleDelete: attributes.onDelete,
                    rowSelection: attributes.rowSelection,
                    readOnly: page.isViewMode,
                }}>
                    {page.isViewMode ? (
                        <ViewProfileForm
                            user={page.user}
                            canEdit={canEditProfile}
                            canSeeRole={canEditRole}
                            onEdit={handleEdit}
                            onOpenCv={handleOpenCv}
                        />
                    ) : (
                        <EditProfileForm
                            user={page.user}
                            roles={page.roles}
                            canEditRole={canEditRole}
                            handleSave={page.handleSaveProfile}
                            onChange={setChangedUser}
                            handleUploadImage={page.handleUploadImage}
                            onOpenCv={handleOpenCv}
                        />
                    )}
                </AttributeContext>
            </ProjectContext>
            {!page.isViewMode && (
                <ProfileAttributeEditModal open={attributes.isEditOpen}
                                           definitions={attributes.openedAttribute ? [attributes.openedAttribute.attributeDefinition] : attributes.definitions}
                                           onClose={attributes.closeEdit}
                                           onSave={attributes.onUpdate}
                                           isEditing
                                           selectedDefinition={attributes.openedAttribute?.attributeDefinition ?? null}
                                           initialValue={attributes.openedAttribute?.value}
                />
            )}
            <AttributeValueModal
                open={attributes.isViewOpen}
                attribute={attributes.viewedAttribute}
                onClose={attributes.closeView}
                loadImage={page.loadAttributeImage}
            />
            {!page.isViewMode && (
                <ProfileAttributeEditModal open={attributes.isCreateOpen}
                                           definitions={attributes.definitions}
                                           onClose={attributes.closeCreate}
                                           onSave={attributes.onCreate}/>
            )}
        </div>
    );
}
