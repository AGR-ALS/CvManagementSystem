import {useEffect, useState} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {Discussion} from "../models/Discussion";
import {GetDiscussion, SendMessage} from "../Services/DiscussionService";
import {GetCurrentUser, GetCurrentUserId} from "../Services/UserService";

export function useDiscussion(positionId: string | null, activeTab: string) {
    const {t} = useTranslation();
    const [discussion, setDiscussion] = useState<Discussion | null>(null);
    const [loading, setLoading] = useState(true);
    const [userId, setUserId] = useState("");
    const [authorNames, setAuthorNames] = useState<Record<string, string>>({});
    useEffect(() => {
        const load = async () => {
            if(activeTab !== "discussion") return;
            if (!positionId) return setLoading(false);
            setLoading(true);
            try {
                const [currentUserId, data] = await Promise.all([GetCurrentUserId(), GetDiscussion(positionId)]);
                setUserId(currentUserId);
                setDiscussion(data);
                const users = await Promise.all([...new Set(data.messages.map(item  => item.userId))].map(async id => {
                    try {
                        const user = await GetCurrentUser(id);
                        return [id, `${user.profileData?.firstName ?? ""} ${user.profileData?.lastName ?? ""}`.trim() || t("app.unnamedUser")] as const;
                    } catch {
                        return [id, t("app.unnamedUser")] as const;
                    }
                }));
                setAuthorNames(Object.fromEntries(users));
            } catch {
                message.error(t("messages.discussionLoadError"));
            } finally {
                setLoading(false);
            }
        };
        load();
    }, [positionId, activeTab]);
    const onSend = async (text: string) => {
        if (!discussion || !userId || !positionId) return;
        try {
            await SendMessage({text, discussionId: discussion.id, userId});
            setDiscussion(await GetDiscussion(positionId));
        } catch {
            message.error(t("messages.discussionSendError"));
        }
    };
    return {discussion, loading, authorNames, onSend, setDiscussion};
}
