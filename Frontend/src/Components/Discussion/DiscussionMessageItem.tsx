import {useTranslation} from "react-i18next";
import {Link} from "react-router";
import {DiscussionMessage} from "../../models/Discussion";
import ReactMarkdown from "react-markdown";

interface Props {
    message: DiscussionMessage;
    authorName?: string;
}

export default function DiscussionMessageItem({message, authorName}: Props) {
    const {t} = useTranslation();
    const displayName = authorName || t("app.unnamedUser");
    return <div className="card shadow-sm border-0 bg-light">
        <div
            className="card-header bg-transparent border-bottom-0 d-flex justify-content-between align-items-center py-2">
            <Link to={`/profile?userId=${encodeURIComponent(message.userId)}&mode=view`}
                  className="fw-bold text-primary text-decoration-none">{displayName}</Link>
            <span className="text-muted small">{new Date(message.sentAt).toLocaleString()}</span>
        </div>
        <div className="card-body py-2"><ReactMarkdown>{message.text}</ReactMarkdown></div>
    </div>;
}
