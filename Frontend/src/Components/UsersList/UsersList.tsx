import React, {useMemo} from "react";
import {TableColumnsType, TableProps} from "antd";
import {useTranslation} from "react-i18next";
import GenericTable from "../CommonTable/CommonTable";
import {UserProfile} from "../../models/UserProfile";

interface Props {
    data: UserProfile[];
    handleOpen: (user: UserProfile) => void;
    rowSelection?: TableProps<UserProfile>['rowSelection'];
}

export default function UsersList({data, handleOpen, rowSelection}: Props) {
    const {t} = useTranslation();

    const columns: TableColumnsType<UserProfile> = useMemo(() => [
        {
            title: t("users.name"),
            key: "name",
            render: (_, record) => `${record.profileData?.firstName || ""} ${record.profileData?.lastName || ""}`,
            width: "30%",
        },
        {
            title: t("users.role"),
            key: "role",
            width: "20%",
            render: (_, record) => record.role?.name || "",
        },
        {
            title: t("users.email"),
            dataIndex: "email",
            key: "email",
            width: "30%",
        },
        {
            title: t("users.blocked"),
            dataIndex: "isBlocked",
            key: "isBlocked",
            render: (value: boolean) => value ? t("app.yes") : t("app.no"),
            width: "20%",
        },
    ], [t]);

    return (
        <GenericTable<UserProfile>
            dataSource={data}
            columns={columns}
            rowKey="id"
            onRowClick={handleOpen}
            rowSelection={rowSelection}
        />
    );
}
