import {useState} from "react";
import {message, TableProps} from "antd";
import {useTranslation} from "react-i18next";
import {AttributeValue} from "../models/AttributeValue";
import {AttributeDefinition} from "../models/AttributeDefinition";
import {AttributeDataType} from "../models/AttributeDataType";
import {CreateUpdateAttributeValueRequest} from "../models/CreateUpdateAttributeValueRequest";
import {
    AddAttributeValueToUser,
    DeleteAttributeValues,
    GetAttributeDefinitions,
    GetUserAttributes,
    UpdateAttributeValueToUser
} from "../Services/AttributeService";

export function useProfileAttributes(userId: string, values: AttributeValue[], onValuesChange: (values: AttributeValue[]) => void, readOnly: boolean) {
    const {t} = useTranslation();
    const [selectedKeys, setSelectedKeys] = useState<React.Key[]>([]);
    const [definitions, setDefinitions] = useState<AttributeDefinition[]>([]);
    const [openedAttribute, setOpenedAttribute] = useState<AttributeValue | null>(null);
    const [viewedAttribute, setViewedAttribute] = useState<AttributeValue | null>(null);
    const [isCreateOpen, setIsCreateOpen] = useState(false);
    const [isEditOpen, setIsEditOpen] = useState(false);
    const [isViewOpen, setIsViewOpen] = useState(false);
    const refresh = async () => onValuesChange(await GetUserAttributes(userId));
    const onOpen = (attribute: AttributeValue) => {
        if (readOnly) {
            setViewedAttribute(attribute);
            setIsViewOpen(true);
        } else {
            setOpenedAttribute(attribute);
            setIsEditOpen(true);
        }
    };
    const onAdd = async () => {
        setDefinitions(await GetAttributeDefinitions());
        setIsCreateOpen(true);
    };
    const onDelete = async (ids: React.Key[]) => {
        try {
            await DeleteAttributeValues(ids.map(String));
            await refresh();
            setSelectedKeys([]);
            message.success(t("messages.profileAttrDeleted"));
        } catch {
            message.error(t("messages.profileAttrDeleteError"));
        }
    };
    const onCreate = async (type: AttributeDataType, data: CreateUpdateAttributeValueRequest) => {
        try {
            await AddAttributeValueToUser(userId, type, data);
            await refresh();
            setIsCreateOpen(false);
            message.success(t("messages.profileAttrCreated"));
        } catch {
            message.error(t("messages.profileAttrCreateError"));
        }
    };
    const onUpdate = async (type: AttributeDataType, data: CreateUpdateAttributeValueRequest) => {
        if (!openedAttribute) return;
        try {
            await UpdateAttributeValueToUser(openedAttribute.id, type, data);
            await refresh();
            setIsEditOpen(false);
            setOpenedAttribute(null);
            message.success(t("messages.profileAttrUpdated"));
        } catch {
            message.error(t("messages.profileAttrUpdateError"));
        }
    };
    const rowSelection: TableProps<AttributeValue>["rowSelection"] = {
        selectedRowKeys: selectedKeys,
        onChange: setSelectedKeys
    };
    return {
        definitions,
        openedAttribute,
        viewedAttribute,
        isCreateOpen,
        isEditOpen,
        isViewOpen,
        rowSelection,
        onOpen,
        onAdd,
        onDelete,
        onCreate,
        onUpdate,
        closeCreate: () => setIsCreateOpen(false),
        closeEdit: () => {
            setIsEditOpen(false);
            setOpenedAttribute(null);
        },
        closeView: () => {
            setIsViewOpen(false);
            setViewedAttribute(null);
        }
    };
}
