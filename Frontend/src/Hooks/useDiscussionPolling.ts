import {useEffect} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {GetDiscussion} from "../Services/DiscussionService";
import {useDiscussion} from "./useDiscussion";
import {CONFIG} from "../config";

export function useDiscussionPolling(
    positionId: string | null,
    activeTab: string,
    intervalMs = CONFIG.DISCUSSION_POLL_INTERVAL,
) {
    const {t} = useTranslation();
    const {discussion, loading, authorNames, onSend, setDiscussion} = useDiscussion(positionId, activeTab);

    useEffect(() => {
        if (activeTab !== "discussion" || !positionId) return;

        const interval = setInterval(async () => {
            try {
                const data = await GetDiscussion(positionId);
                setDiscussion((prev) => {
                    if (
                        prev &&
                        prev.messages.length === data.messages.length &&
                        prev.messages.at(-1)?.id === data.messages.at(-1)?.id
                    ) {
                        return prev;
                    }
                    return data;
                });
            } catch {
                message.error(t("messages.discussionLoadError"));
            }
        }, intervalMs);

        return () => clearInterval(interval);
    }, [positionId, activeTab]);

    return {discussion, loading, authorNames, onSend};
}
