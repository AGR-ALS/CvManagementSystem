import {AttributeValue} from "../../models/AttributeValue";
import {Button, TableColumnsType, Typography} from "antd";
import {useTranslation} from "react-i18next";
import GenericTable from "../CommonTable/CommonTable";
import {useAttributeContext} from "../../Contexts/AttributeContext";
import {Toolbar} from "../Toolbar/Toolbar";
import "./ProfileAttributeList.css";

export default function ProfileAttributesTable() {
    const {t} = useTranslation();
    const {data, handleOpen, rowSelection, handleAdd, handleDelete, readOnly} = useAttributeContext();

    const formatValue = (value: AttributeValue['value']) => {
        if (value == null) {
            return "";
        }

        if (typeof value === "object") {
            if ("oneOfManyValueId" in value && value.value != null) {
                return value.value;
            }
            if ("start" in value && "end" in value) {
                return `${value.start || ""} - ${value.end || ""}`;
            }
            return JSON.stringify(value);
        }

        return String(value);
    };

    const columns: TableColumnsType<AttributeValue> = [
        {
            title: t("profileAttribute.attributeName"),
            key: "attributeName",
            width: "40%",
            render: (_, record) => record.attributeDefinition.name,
        },
        {
            title: t("profileAttribute.attributeValue"),
            key: "attributeValue",
            width: "60%",
            render: (_, record) => {
                const dataType = record.attributeDefinition.dataType;

                if (dataType === 2 || dataType === 1) {
                    return (
                        <Button
                            onClick={(e) => {
                                e.stopPropagation();
                                handleOpen(record);
                            }}
                        >
                            {t("profileAttribute.viewValue")}
                        </Button>
                    );
                }

                const valueText = formatValue(record.value);
                return (
                    <Typography.Text
                        ellipsis={{tooltip: valueText}}
                    >
                        {valueText}
                    </Typography.Text>
                );
            },
        },
    ];

    return (
        <div className="profile-attr-list__wrapper">
            {!readOnly && (
                <div className="profile-attr-list__toolbar">
                    <Toolbar
                        selectedRowKeys={rowSelection?.selectedRowKeys ?? []}
                        onCreate={handleAdd}
                        onDelete={(ids) => handleDelete(ids)}
                    />
                </div>
            )}
            <div className="profile-attr-list__content">
                <GenericTable<AttributeValue>
                    dataSource={data}
                    columns={columns}
                    rowKey="id"
                    rowSelection={readOnly ? undefined : rowSelection}
                    onRowClick={readOnly ? undefined : handleOpen}
                />
            </div>
        </div>
    );
}
