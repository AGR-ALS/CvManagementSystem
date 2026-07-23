import {useTranslation} from "react-i18next";
import {Alert, Spin} from "antd";
import DiscussionList from "./DiscussionList";
import MessageInput from "./MessageInput";
import {Discussion} from "../../models/Discussion";
import "./DiscussionView.css";

interface Props {
    positionId: string | null;
    discussion: Discussion | null;
    loading: boolean;
    authorNames: Record<string, string>;
    onSend: (text: string) => Promise<void>;
}

export default function DiscussionView({positionId, discussion, loading, authorNames, onSend}: Props) {
    const {t} = useTranslation();
    if (!positionId) return <Alert message={t("discussion.saveFirst")} type="info"
                                   className="m-3" showIcon/>;
    if (loading) return <div className="position-fixed top-50 start-50 translate-middle"><Spin
        size="large"/></div>;
    return <div className="d-flex flex-column discussion-view__container">
        <div className="flex-grow-1 overflow-auto p-3 bg-white border rounded mb-3 shadow-sm">
            {discussion && discussion.messages.length > 0 ?
                <DiscussionList messages={discussion.messages} authorNames={authorNames}/> :
                <div className="text-center text-muted mt-5"><p>{t("discussion.noMessages")}</p></div>}
        </div>
        <MessageInput onSend={onSend} disabled={!discussion}/>
    </div>;
}
