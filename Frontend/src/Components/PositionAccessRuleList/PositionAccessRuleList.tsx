import {TableColumnsType, TableProps} from "antd";
import GenericTable from "../CommonTable/CommonTable";
import {AccessRule} from "../../models/AccessRule";
import {Toolbar} from "../Toolbar/Toolbar";
import {FilterOperator} from "../../models/FilterOperator";
import React, {useMemo, useState} from "react";
import {useTranslation} from "react-i18next";
import "./PositionAccessRuleList.css";

const filterOperatorLabel = (op: FilterOperator, t: (key: string) => string): string =>
    [t("operators.equal"), t("operators.notEqual"), t("operators.greaterThan"), t("operators.lessThan"), t("operators.greaterThanOrEqual"), t("operators.lessThanOrEqual"), t("operators.contains"), t("operators.notContains"), t("operators.has"), t("operators.intersects")][op] ?? String(op);

interface Props {
    data: AccessRule[];
    handleOpen: (rule: AccessRule) => void;
    handleAdd: () => void;
    handleDelete: (ids: React.Key[]) => void;
    readOnly?: boolean;
}

export default function PositionAccessRuleList({data, handleOpen, handleAdd, handleDelete, readOnly = false}: Props) {
    const {t} = useTranslation();
    const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);

    const rowSelection: TableProps<AccessRule>['rowSelection'] = readOnly
        ? undefined
        : {
            selectedRowKeys,
            onChange: (keys) => setSelectedRowKeys(keys),
        };

    const columns: TableColumnsType<AccessRule> = useMemo(() => [
        {
            title: t("accessRule.attributeName"),
            key: "attributeName",
            width: "30%",
            render: (_, record) => record.attributeValue.attributeDefinition.name,
        },
        {
            title: t("accessRule.filterOperator"),
            key: "filterOperator",
            width: "20%",
            render: (_, record) => filterOperatorLabel(record.filterOperator, t),
        },
        {
            title: t("accessRule.value"),
            key: "attributeValue",
            width: "50%",
            render: (_, record) => {
                const val = record.attributeValue.value;
                if (typeof val === 'object' && val !== null) {
                    if ('start' in val) {
                        return `${(val as any).start} - ${(val as any).end}`;
                    }
                    if ('value' in val && 'oneOfManyValueId' in val) {
                        return (val as any).value;
                    }
                }
                return String(val);
            },
        },
    ], [t]);

    return (
        <div className="position-access-rule-list__wrapper">
            {!readOnly && (
                <div className="position-access-rule-list__toolbar">
                    <Toolbar
                        selectedRowKeys={selectedRowKeys}
                        onCreate={handleAdd}
                        onDelete={handleDelete}
                    />
                </div>
            )}
            <div className="position-access-rule-list__content">
                <GenericTable<AccessRule>
                    dataSource={data}
                    columns={columns}
                    rowKey="id"
                    onRowClick={readOnly ? undefined : handleOpen}
                    rowSelection={rowSelection}
                />
            </div>
        </div>
    );
}
