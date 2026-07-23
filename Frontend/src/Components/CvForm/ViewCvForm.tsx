import {Button, Divider, Image, Typography} from "antd";
import {useTranslation} from "react-i18next";
import {useCvFormContext} from "../../Contexts/CvFormContext";
import CvAttributeTable from "../CvAttributeTable/CvAttributeTable";
import CvProjectList from "../CvProjectList/CvProjectList";
import "../../styles/card-wrapper.css";
import "./CvForm.css";

export default function ViewCvForm() {
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
                    onRowClick={ctx.isViewMode ? undefined : ctx.openAttribute}
                    loadImage={ctx.loadAttributeImage}/>
                <Divider orientation="horizontal" className="mt-4">{t("cv.projects")}</Divider>
                <CvProjectList
                    selectedProjects={ctx.projects}
                    maxProjects={cv.position.maxProjects}
                    onRemove={ctx.isViewMode ? undefined : ctx.removeProject}
                    onAdd={ctx.isViewMode ? undefined : ctx.openSelector}/>
                <div className="cv-form__button-row--view">
                    <div>
                        {!ctx.regular && (
                            <span onClick={ctx.toggleLike} className="cv-form__like">
                                <i className={`${ctx.liked ? "bi bi-heart-fill" : "bi bi-heart"} text-danger me-2`}></i>
                                {cv.likes}
                            </span>
                        )}
                    </div>
                    <div>
                        {ctx.canEdit && !cv.published && (
                            <Button onClick={ctx.publish} className="me-2">{t("cv.publish")}</Button>
                        )}
                        {ctx.canEdit && (
                            <Button type="primary" onClick={ctx.onEdit}>{t("app.edit")}</Button>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}
