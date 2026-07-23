import {useMemo} from "react";
import {useTranslation} from "react-i18next";
import {AttributeDefinition} from "../../models/AttributeDefinition";
import GenericTable from "../CommonTable/CommonTable";
import {TableColumnsType, TableProps} from "antd";

interface Props {
    data: AttributeDefinition[];
    handleOpen: (position: AttributeDefinition) => void;
    rowSelection?: TableProps<AttributeDefinition>['rowSelection'];
}

export default function AttributeDefinitionList({data, handleOpen, rowSelection}: Props) {
    const {t} = useTranslation();

    const columns: TableColumnsType<AttributeDefinition> = useMemo(() => [
        {
            title: t("attributeDefinition.title"),
            dataIndex: 'name',
            key: 'name',
            width: '33%',
        },
        {
            title: t("attributeDefinition.attributeCategory"),
            key: 'attributeCategoryId',
            width: '33%',
            render: (_, record) => record.attributeCategory?.name || record.attributeCategoryId,
        },
        {
            title: t("attributeDefinition.attributeType"),
            dataIndex: 'dataType',
            key: 'dataType',
            width: '34%',
            render: (value: number) => {
                const labels = [
                    t("dataTypes.string"),
                    t("dataTypes.text"),
                    t("dataTypes.image"),
                    t("dataTypes.numeric"),
                    t("dataTypes.date"),
                    t("dataTypes.period"),
                    t("dataTypes.boolean"),
                    t("dataTypes.oneOfMany")
                ];
                return labels[value] ?? value;
            },
        },
    ], [t]);

    return (
        <GenericTable<AttributeDefinition>
            dataSource={data}
            columns={columns}
            rowKey="id"
            onRowClick={handleOpen}
            rowSelection={rowSelection}
        />
    );
}