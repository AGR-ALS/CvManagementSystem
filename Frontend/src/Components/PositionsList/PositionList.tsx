import {TableColumnsType, TableProps} from "antd";
import React, {useMemo} from "react";
import {useTranslation} from "react-i18next";
import {Position} from "../../models/Position";
import {ExpertiseLevel} from "../../models/ExpertiseLevel";
import GenericTable from "../CommonTable/CommonTable";

interface Props {
    data: Position[];
    handleOpen: (position: Position) => void;
    rowSelection?: TableProps<Position>['rowSelection'];
}

const expertiseLevelLabel = (level: ExpertiseLevel, t: (key: string) => string): string =>
    [t("position.junior"), t("position.middle"), t("position.senior")][level] ?? String(level);

export default function PositionList({data, handleOpen, rowSelection}: Props) {
    const {t} = useTranslation();

    const columns: TableColumnsType<Position> = useMemo(() => [
        {
            title: t("positionList.title"),
            dataIndex: 'title',
            key: 'title',
            width: '25%',
        },
        {
            title: t("positionList.description"),
            dataIndex: 'description',
            key: 'description',
            width: '25%',
            ellipsis: true,
            render: (description: string) => (
                <span
                    title={description}
                    className="d-block text-truncate"
                >
                    {description}
                </span>
            ),
        },
        {
            title: t("positionList.expertiseLevel"),
            key: 'expertiseLevel',
            width: '20%',
            render: (_, record) => expertiseLevelLabel(record.expertiseLevel, t),
        },
        {
            title: t("positionList.createdAt"),
            dataIndex: 'createdAt',
            key: 'createdAt',
            width: '30%',
            render: (value: string) => {
                if (!value) return t("app.noValue");
                return new Date(value).toLocaleString();
            },
        },
    ], [t]);

    return (
        <GenericTable<Position>
            dataSource={data}
            columns={columns}
            rowKey="id"
            onRowClick={handleOpen}
            rowSelection={rowSelection}
        />
    );
}