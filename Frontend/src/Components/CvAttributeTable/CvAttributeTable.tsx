import {Button, Typography} from "antd";
import {useCallback, useState} from "react";
import {useTranslation} from "react-i18next";
import GenericTable from "../CommonTable/CommonTable";
import {AccessRule} from "../../models/AccessRule";
import {AttributeValue} from "../../models/AttributeValue";
import {AttributeDefinition} from "../../models/AttributeDefinition";
import {AttributeDataType} from "../../models/AttributeDataType";
import AttributeValueModal from "../ProfileAttributeModal/ProfileAttributeModal";
import {ImageUrlResponse} from "../../models/ImageUrl";

export interface CvAttributeRow {
    key: string;
    definition: AttributeDefinition | null;
    accessRule: AccessRule;
    userAttribute?: AttributeValue;
    missing: boolean;
}

interface Props {
    accessRules: AccessRule[];
    userAttributes: AttributeValue[];
    onRowClick?: (row: CvAttributeRow) => void;
    loadImage: (key: string) => Promise<string>;
}

const formatValue = (value: any) => {
    if (value == null) {
        return "";
    }

    if (typeof value === "object") {
        if ("oneOfManyValueId" in value && value.value != null) {
            return String(value.value);
        }
        if ("start" in value && "end" in value) {
            return `${value.start || ""} - ${value.end || ""}`;
        }
        return JSON.stringify(value);
    }

    return String(value);
};

const canPreview = (dataType?: AttributeDataType) =>
    dataType === AttributeDataType.Text || dataType === AttributeDataType.Image;

export default function CvAttributeTable({accessRules, userAttributes, onRowClick, loadImage}: Props) {
    const {t} = useTranslation();
    const [viewingAttribute, setViewingAttribute] = useState<AttributeValue | null>(null);
    const loadImageForModal = useCallback((key: string) =>
        loadImage(key).then(url => ({url}) as ImageUrlResponse), [loadImage]);
    const rows: CvAttributeRow[] = accessRules.map((rule) => {
        const definition = rule.attributeValue.attributeDefinition || null;
        const userAttribute = definition
            ? userAttributes.find((item) => item.attributeDefinition.id === definition.id)
            : undefined;

        return {
            key: rule.id,
            definition,
            accessRule: rule,
            userAttribute,
            missing: !userAttribute,
        };
    });

    const columns = [
        {
            title: t("cvAttribute.attributeDefinition"),
            key: "definition",
            width: "40%",
            render: (_: any, record: CvAttributeRow) => record.definition?.name || t("cvAttribute.unknown"),
        },
        {
            title: t("cvAttribute.value"),
            key: "value",
            width: "60%",
            render: (_: any, record: CvAttributeRow) => {
                if (!record.userAttribute) {
                    return <span>&nbsp;</span>;
                }
                if (canPreview(record.definition?.dataType)) {
                    return (
                        <Button
                            onClick={(e) => {
                                e.stopPropagation();
                                setViewingAttribute(record.userAttribute!);
                            }}
                        >
                            {t("cvAttribute.viewValue")}
                        </Button>
                    );
                }
                const valueText = formatValue(record.userAttribute.value);
                return (
                    <Typography.Text ellipsis={{tooltip: valueText}}>
                        {valueText}
                    </Typography.Text>
                );
            },
        },
    ];

    return (
        <>
            <GenericTable<CvAttributeRow>
                dataSource={rows}
                columns={columns}
                rowKey="key"
                onRowClick={onRowClick}
                rowStyle={(record) =>
                    record.missing
                        ? {
                            backgroundColor: "var(--bs-danger-bg-subt, #fff1f0)",
                        }
                        : undefined
                }
            />
            <AttributeValueModal
                open={viewingAttribute !== null}
                attribute={viewingAttribute}
                onClose={() => setViewingAttribute(null)}
                loadImage={loadImageForModal}
            />
        </>
    );
}
