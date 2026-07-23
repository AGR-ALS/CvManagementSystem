import {Button, Divider, Typography} from "antd";
import {useTranslation} from "react-i18next";
import {Position} from "../../models/Position";
import PositionAccessRuleList from "../PositionAccessRuleList/PositionAccessRuleList";
import {usePositionFormContext} from "../../Contexts/PositionFormContext";
import {ExpertiseLevel} from "../../models/ExpertiseLevel";
import "../../styles/card-wrapper.css";
import "./PositionForm.css";

interface Props {
    position: Position | null;
    canEdit?: boolean;
    onEdit?: () => void;
}

export default function ViewPositionForm({position, canEdit = false, onEdit}: Props) {
    const {t} = useTranslation();
    const {onGenerateCv} = usePositionFormContext();

    const field = (label: string, value?: string | number | null, multiline = false) => (
        <div className="position-form__field">
            <Typography.Text type="secondary">{label}</Typography.Text>
            <div className={multiline ? "position-form__field-text" : undefined}>{value ?? t("app.noValue")}</div>
        </div>
    );

    const level = [t("position.junior"), t("position.middle"), t("position.senior")][position?.expertiseLevel ?? ExpertiseLevel.Junior];

    return (
        <div className="d-flex justify-content-center position-form__outer">
            <div className="card-wrapper card-wrapper--80">
                {field(t("position.title"), position?.title)}
                {field(t("position.description"), position?.description, true)}
                <div className="position-form__flex-row">
                    <div className="position-form__flex-col">{field(t("position.maxProjects"), position?.maxProjects)}</div>
                    <div
                        className="position-form__flex-grow">{field(t("position.technologies"), position?.technologies?.map(t => t.name).join(", ") || t("app.noValue"))}</div>
                </div>
                <div className="position-form__flex-row">
                    <div className="position-form__flex-col">{field(t("position.expertiseLevel"), level)}</div>
                    <div
                        className="position-form__flex-grow">{field(t("position.restrictedAccess"), position?.restricted ? t("app.yes") : t("app.no"))}</div>
                </div>
                <Divider>{t("position.accessRules")}</Divider>
                <PositionAccessRuleList data={position?.accessRules || []}
                                        handleOpen={() => {}}
                                        handleAdd={() => {}}
                                        handleDelete={() => {}}
                                        readOnly/>
                <Divider/>
                <div className="position-form__actions">
                    {position && <Button onClick={onGenerateCv}>{t("position.generateCv")}</Button>}
                    {canEdit && onEdit && <Button type="primary" onClick={onEdit}>{t("app.edit")}</Button>}
                </div>
            </div>
        </div>
    );
}
