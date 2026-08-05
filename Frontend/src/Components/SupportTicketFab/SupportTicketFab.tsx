import {useTranslation} from "react-i18next";
import "./SupportTicketFab.css";

interface Props {
    onClick: () => void;
}

export default function SupportTicketFab({onClick}: Props) {
    const {t} = useTranslation();

    return (
        <button
            type="button"
            className="btn btn-primary rounded-circle support-ticket-fab"
            onClick={onClick}
            aria-label={t("supportTicket.title")}
        >
            <i className="bi bi-question-lg"/>
        </button>
    );
}
