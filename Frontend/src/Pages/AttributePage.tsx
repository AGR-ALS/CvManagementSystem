import {message} from "antd";
import {useTranslation} from "react-i18next";
import {AttributeDefinition} from "../models/AttributeDefinition";
import {AttributeCategory} from "../models/AttributeCategory";
import {AttributeDataType} from "../models/AttributeDataType";
import AttributeDefinitionList from "../Components/AttributeDefinitionList/AttributeDefinitionList";
import CommonPage from "./CommonPage";
import {
    CreateAttributeDefinitions,
    DeleteAttributeDefinitions,
    EditAttributeDefinitions,
    GetAttributeCategories,
    GetAttributeDefinitions
} from "../Services/AttributeService";
import React, {useEffect, useState} from "react";
import AttributeEditModal from "../Components/AttributeEditModal/AttributeEditModal";

const defaultValues: AttributeDefinition = {
    id: "",
    name: "",
    attributeCategoryId: "",
    dataType: AttributeDataType.String,
    oneOfManyOptions: null,
}
export default function AttributeDefinitionPage() {
    const {t} = useTranslation();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentAttribute, setCurrentAttribute] = useState<AttributeDefinition | null>(null);
    const [modalCreatingMode, setModalCreatingMode] = useState<boolean>(true);
    const [entities, setEntities] = useState<AttributeDefinition[]>([]);
    const [categories, setCategories] = useState<AttributeCategory[]>([]);
    const handleCreate = () => {
        setModalCreatingMode(true);
        setCurrentAttribute(null);
        setIsModalOpen(true);
    };

    const handleDelete = async (ids: React.Key[]) => {
        if (ids.length === 0) return;

        const stringIds = ids.map(id => String(id));

        try {
            await DeleteAttributeDefinitions(stringIds);
            setEntities(await GetAttributeDefinitions());
            message.success(t("messages.attributesDeleted"));
        } catch {
            message.error(t("messages.attributesDeleteError"));
        }
    };

    const handleRowClick = (attribute: AttributeDefinition) => {
        setModalCreatingMode(false);
        setCurrentAttribute(attribute);
        setIsModalOpen(true);
    };

    const handleCancel = () => {
        setIsModalOpen(false);
        setCurrentAttribute(null);
    };

    const handleSave = async (values: AttributeDefinition) => {
        try {
            if (modalCreatingMode) {
                await CreateAttributeDefinitions(values);
            } else {
                await EditAttributeDefinitions(values);
            }
            let attributesPromise = GetAttributeDefinitions();
            setIsModalOpen(false);
            setCurrentAttribute(null);
            setEntities(await attributesPromise);
            message.success(t("messages.attributeSaved"));
        } catch {
            message.error(t("messages.attributeSaveError"));
        }
    };

    useEffect(() => {
        const load = async () => {
            try {
                setEntities(await GetAttributeDefinitions());
                setCategories(await GetAttributeCategories());
            } catch {
                message.error(t("messages.attributesLoadError"));
            }
        };
        load();
    }, []);

    return (
        <div>
            <CommonPage
                data={entities}
                TableComponent={AttributeDefinitionList}
                onCreate={handleCreate}
                onDelete={handleDelete}
                onRowClick={handleRowClick}
            />
            <AttributeEditModal
                open={isModalOpen}
                attribute={currentAttribute}
                categories={categories}
                onCancel={handleCancel}
                onSave={handleSave}
                isCreating={modalCreatingMode}
            />
        </div>
    )
}