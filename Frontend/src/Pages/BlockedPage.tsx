import {useTranslation} from "react-i18next";
import {Button, Card, Typography} from "antd";
import {useNavigate} from "react-router";
import "./BlockedPage.css";

export default function BlockedPage() {
    const {t} = useTranslation();
    const navigate = useNavigate();

    return (
        <div className="blocked-page__wrapper">
            <Card className="blocked-page__card">
                <i className="bi bi-exclamation-triangle fs-1 text-danger"/>
                <Typography.Title level={3} className="mt-3">
                    {t("blocked.title")}
                </Typography.Title>
                <Typography.Text type="secondary">
                    {t("blocked.description")}
                </Typography.Text>
                <div className="mt-4">
                    <Button type="primary" onClick={() => navigate("/authentication")}>
                        {t("auth.goToLogin")}
                    </Button>
                </div>
            </Card>
        </div>
    );
}
