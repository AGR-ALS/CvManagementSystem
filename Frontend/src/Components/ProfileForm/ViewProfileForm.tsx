import {Button, Divider, Image, Typography} from "antd";
import {useTranslation} from "react-i18next";
import {UserProfile} from "../../models/UserProfile";
import ProjectList from "../ProjectList/ProjectList";
import ProfileAttributesTable from "../ProfileAttributeList/ProfileAttributeList";
import CvListTable from "../CvListTable/CvListTable";
import "../../styles/card-wrapper.css";
import "./ProfileForm.css";

interface Props {
    user: UserProfile;
    canEdit: boolean;
    canSeeRole: boolean;
    onEdit: () => void;
    onOpenCv: (positionId: string) => void;
}

export default function ViewProfileForm({user, canEdit, canSeeRole, onEdit, onOpenCv}: Props) {
    const {t} = useTranslation();

    const renderField = (label: string, value?: string | number | null) => (
        <div className="mb-3">
            <Typography.Text type="secondary">{label}</Typography.Text>
            <div>{value ?? t("app.noValue")}</div>
        </div>
    );

    return (
        <div className="d-flex justify-content-center profile-form__outer">
            <div className="card-wrapper card-wrapper--60">
                <div className="row g-3">
                    <div className="col-md-9">
                        <div className="row g-3">
                            <div className="col-md-6">
                                {renderField(t("profile.firstName"), user.profileData?.firstName)}
                            </div>
                            <div className="col-md-6">
                                {renderField(t("profile.lastName"), user.profileData?.lastName)}
                            </div>
                            <div className="col-md-6">
                                {renderField(t("profile.location"), user.profileData?.location)}
                            </div>
                            <div className="col-md-6">
                                {renderField(t("profile.email"), user.email)}
                            </div>
                        </div>
                        {canSeeRole && renderField(t("profile.role"), user.role?.name)}
                    </div>
                    {user.profileData?.personalPhotoUrl && (
                        <div className="col-md-3 d-flex justify-content-center align-items-center">
                            <Image width={160} alt={t("profile.profileAlt")} src={user.profileData.personalPhotoUrl}/>
                        </div>
                    )}
                </div>

                <Divider orientation="horizontal">
                    {t("profile.projects")}
                </Divider>

                <ProjectList/>

                <Divider className="mb-4"/>

                <Divider orientation="horizontal" className="mt-4">
                    {t("profile.attributes")}
                </Divider>

                <ProfileAttributesTable/>
                <Divider className="mb-4"/>

                <Divider orientation="horizontal" className="mt-4">
                    {t("profile.cvs")}
                </Divider>
                <div className="my-3">
                    <CvListTable data={user.cvs} handleOpen={(cv) => onOpenCv(cv.positionId)}/>
                </div>

                {canEdit && (
                    <div className="d-flex justify-content-end">
                        <Button type="primary" onClick={onEdit}>
                            {t("profile.edit")}
                        </Button>
                    </div>
                )}
            </div>
        </div>
    );
}
