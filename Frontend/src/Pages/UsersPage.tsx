import {message} from "antd";
import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router";
import CommonPage from "./CommonPage";
import UsersList from "../Components/UsersList/UsersList";
import {UserProfile} from "../models/UserProfile";
import {BlockUsers, DeleteUsers, GetAllUsers, UnblockUsers} from "../Services/UserService";

export default function UsersPage() {
    const {t} = useTranslation();
    const [users, setUsers] = useState<UserProfile[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        const load = async () => {
            try {
                setUsers(await GetAllUsers());
            } catch {
                message.error(t("messages.usersLoadError"));
            }
        };
        load();
    }, []);

    const refreshUsers = async () => {
        setUsers(await GetAllUsers());
    };

    const handleDelete = async (ids: React.Key[]) => {
        if (ids.length === 0) return;
        const stringIds = ids.map(String);
        try {
            await DeleteUsers(stringIds);
            await refreshUsers();
            message.success(t("messages.usersDeleted"));
        } catch {
            message.error(t("messages.usersDeleteError"));
        }
    };

    const handleBlock = async (ids: React.Key[]) => {
        if (ids.length === 0) return;
        const stringIds = ids.map(String);
        try {
            await BlockUsers(stringIds);
            await refreshUsers();
            message.success(t("messages.usersBlocked"));
        } catch {
            message.error(t("messages.usersBlockError"));
        }
    };

    const handleUnlock = async (ids: React.Key[]) => {
        if (ids.length === 0) return;
        const stringIds = ids.map(String);
        try {
            await UnblockUsers(stringIds);
            await refreshUsers();
            message.success(t("messages.usersUnlocked"));
        } catch {
            message.error(t("messages.usersUnlockError"));
        }
    };

    const handleRowClick = (user: UserProfile) => {
        navigate(`/profile?userId=${encodeURIComponent(user.id)}&mode=view`);
    };

    return (
        <CommonPage
            data={users}
            TableComponent={UsersList}
            onDelete={handleDelete}
            onBlock={handleBlock}
            onUnlock={handleUnlock}
            onRowClick={handleRowClick}
        />
    );
}
