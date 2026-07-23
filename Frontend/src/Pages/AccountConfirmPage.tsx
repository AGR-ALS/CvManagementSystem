import {useTranslation} from "react-i18next";
import {Button, Card, Typography} from "antd";
import {useNavigate} from "react-router";
import "./AccountConfirmPage.css";

export default function AccountConfirmPage() {
    const {t} = useTranslation();
    const navigate = useNavigate();

    return (
        <div className="account-confirm-page__wrapper">
            <Card className="account-confirm-page__card">
                <i className="bi bi-check-circle-fill fs-1 text-success"/>
                <Typography.Title level={3} className="mt-3">
                    {t("accountConfirm.title")}
                </Typography.Title>
                <Typography.Text type="secondary">
                    {t("accountConfirm.description")}
                </Typography.Text>
                <div className="mt-4">
                    <Button type="primary" onClick={() => navigate("/")}>
                        {t("accountConfirm.toMain")}
                    </Button>
                </div>
            </Card>
        </div>
    );
}
