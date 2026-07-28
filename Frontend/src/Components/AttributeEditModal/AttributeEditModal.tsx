import {useEffect, useState} from "react";
import {Button, Form, Input, Modal, Select} from "antd";
import {useTranslation} from "react-i18next";
import {AttributeDataType} from "../../models/AttributeDataType";
import {AttributeCategory} from "../../models/AttributeCategory";
import {AttributeDefinition} from "../../models/AttributeDefinition";
import {OneOfManyOption} from "../../models/OneOfManyOption";

interface Props {
    open: boolean;
    attribute: AttributeDefinition | null;
    categories: AttributeCategory[];
    onCancel: () => void;
    onSave: (values: AttributeDefinition) => void;
    isCreating: boolean;
}

export default function AttributeEditModal({open, attribute, categories, onCancel, onSave, isCreating}: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm<AttributeDefinition>();
    const [tags, setTags] = useState<string[]>([]);
    const dataType = Form.useWatch('dataType', form);

    const isOneOfMany = dataType === AttributeDataType.OneOfMany;

    useEffect(() => {
        if (open) {
            if (isCreating) {
                form.resetFields();
                setTags([]);
            } else if (attribute) {
                form.setFieldsValue(attribute);
                setTags(attribute.oneOfManyOptions?.map(o => o.value) ?? []);
            }
        }
    }, [open, attribute]);

    const handleOk = async () => {
        const values = await form.validateFields();
        const oneOfManyOptions: OneOfManyOption[] | null = isOneOfMany
            ? tags.map(value => {
                const existing = attribute?.oneOfManyOptions?.find(o => o.value === value);
                return existing ?? {value} as OneOfManyOption;
            })
            : null;
        if (isCreating) {
            onSave({...values, oneOfManyOptions});
        } else if (attribute) {
            onSave({...values, id: attribute.id, oneOfManyOptions});
        }
    };

    return (
        <Modal
            title={isCreating ? t("attributeEdit.createTitle") : t("attributeEdit.editTitle")}
            open={open}
            onCancel={onCancel}
            footer={[
                <Button key="cancel" onClick={onCancel}>
                    {t("app.cancel")}
                </Button>,
                <Button key="edit" type="primary" onClick={handleOk}>
                    {isCreating ? t("app.create") : t("app.edit")}
                </Button>,
            ]}
        >
            <Form form={form} layout="vertical">
                <Form.Item
                    label={t("attributeEdit.name")}
                    name="name"
                    rules={[{required: true, message: t("attributeEdit.nameRequired")}]}
                >
                    <Input/>
                </Form.Item>
                <Form.Item
                    label={t("attributeEdit.category")}
                    name="attributeCategoryId"
                    rules={[{required: true, message: t("attributeEdit.categoryRequired")}]}
                >
                    <Select placeholder={t("attributeEdit.categoryPlaceholder")}>
                        {categories.map(category => (
                            <Select.Option key={category.id} value={category.id}>
                                {category.name}
                            </Select.Option>
                        ))}
                    </Select>
                </Form.Item>
                <Form.Item
                    label={t("attributeEdit.dataType")}
                    name="dataType"
                    rules={[{required: true, message: t("attributeEdit.dataTypeRequired")}]}
                >
                    <Select placeholder={t("attributeEdit.dataTypePlaceholder")}>
                        <Select.Option value={AttributeDataType.String}>{t("dataTypes.string")}</Select.Option>
                        <Select.Option value={AttributeDataType.Text}>{t("dataTypes.text")}</Select.Option>
                        <Select.Option value={AttributeDataType.Image}>{t("dataTypes.image")}</Select.Option>
                        <Select.Option value={AttributeDataType.Numeric}>{t("dataTypes.numeric")}</Select.Option>
                        <Select.Option value={AttributeDataType.Date}>{t("dataTypes.date")}</Select.Option>
                        <Select.Option value={AttributeDataType.Period}>{t("dataTypes.period")}</Select.Option>
                        <Select.Option value={AttributeDataType.Boolean}>{t("dataTypes.boolean")}</Select.Option>
                        <Select.Option value={AttributeDataType.OneOfMany}>{t("dataTypes.oneOfMany")}</Select.Option>
                    </Select>
                </Form.Item>
                {isOneOfMany && (
                    <Form.Item label={t("attributeEdit.oneOfManyOptions")}>
                        <Select
                            mode="tags"
                            className="w-100"
                            placeholder={t("attributeEdit.addOptionsPlaceholder")}
                            value={tags}
                            onChange={setTags}
                            tokenSeparators={[',']}
                        />
                    </Form.Item>
                )}
            </Form>
        </Modal>
    );
}