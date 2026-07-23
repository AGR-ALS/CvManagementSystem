import {Button, Divider, Image, Typography} from "antd";
import {useTranslation} from "react-i18next";
import {useCvFormContext} from "../../Contexts/CvFormContext";
import CvAttributeTable from "../CvAttributeTable/CvAttributeTable";
import CvProjectList from "../CvProjectList/CvProjectList";
import CvProjectSelectModal from "../CvProjectSelectModal/CvProjectSelectModal";
import ProfileAttributeEditModal from "../ProfileAttributeEditModal/ProfileAttributeEditModal";
import "../../styles/card-wrapper.css";
import "./CvForm.css";

export default function EditCvForm() {
    const {t} = useTranslation();
    const ctx = useCvFormContext();
    const {cv} = ctx;

    const field = (label: string, value?: string | number) => (
        <div className="cv-form__field">
            <Typography.Text type="secondary">{label}</Typography.Text>
            <div>{value ?? t("app.noValue")}</div>
        </div>
    );

    return (
        <div className="d-flex justify-content-center cv-form__outer">
            <div className="card-wrapper card-wrapper--80">
                <Typography.Title level={4}>{t("cv.applicationFor")}{cv.position.title}</Typography.Title>
                <div className="row g-3">
                    <div className="col-md-8">
                        <Typography.Title level={5}>{t("cv.userInformation")}</Typography.Title>
                        {field(t("cv.firstName"), cv.user.profileData?.firstName)}
                        {field(t("cv.lastName"), cv.user.profileData?.lastName)}
                        {field(t("cv.location"), cv.user.profileData?.location)}
                        {field(t("cv.role"), cv.user.role?.name)}
                        {field(t("cv.email"), cv.user.email)}
                    </div>
                    {ctx.photo && (
                        <div className="col-md-4 d-flex justify-content-center align-items-center">
                            <Image width={200} alt={t("cv.profileAlt")} src={ctx.photo}/>
                        </div>
                    )}
                </div>
                <Divider orientation="horizontal" className="mt-4">{t("cv.attributes")}</Divider>
                <CvAttributeTable
                    accessRules={cv.position.accessRules}
                    userAttributes={cv.user.attributeValues}
                    onRowClick={ctx.openAttribute}
                    loadImage={ctx.loadAttributeImage}/>
                <Divider orientation="horizontal" className="mt-4">{t("cv.projects")}</Divider>
                <CvProjectList
                    selectedProjects={ctx.projects}
                    maxProjects={cv.position.maxProjects}
                    onRemove={ctx.removeProject}
                    onAdd={ctx.openSelector}/>
                <CvProjectSelectModal
                    open={ctx.selectorOpen}
                    projects={cv.user.projects}
                    selectedProjectIds={ctx.projects.map(p => p.id)}
                    maxSelectable={cv.position.maxProjects - ctx.projects.length}
                    onClose={ctx.closeSelector}
                    onSave={ctx.addProjects}/>
                <div className="cv-form__button-row">
                    <Button danger onClick={ctx.remove}>{t("app.delete")}</Button>
                    <div>
                        <Button onClick={ctx.onCancel} className="me-2">{t("app.cancel")}</Button>
                        <Button type="primary" onClick={ctx.save}>{t("app.save")}</Button>
                    </div>
                </div>
                <ProfileAttributeEditModal
                    open={ctx.attributeOpen}
                    definitions={cv.user.attributeValues.map(a => a.attributeDefinition)}
                    onClose={ctx.closeAttribute}
                    onSave={ctx.saveAttribute}
                    isEditing
                    selectedDefinition={ctx.editingRow?.definition ?? undefined}
                    initialValue={ctx.editingRow?.userAttribute?.value}/>
            </div>
        </div>
    );
}
