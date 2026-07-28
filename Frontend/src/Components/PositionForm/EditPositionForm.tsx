import {Button, Checkbox, Divider, Form, Input, InputNumber, Select} from "antd";
import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {Position} from "../../models/Position";
import {AccessRule} from "../../models/AccessRule";
import {AttributeValue} from "../../models/AttributeValue";
import {FilterOperator} from "../../models/FilterOperator";
import {AttributeDataType} from "../../models/AttributeDataType";
import {CreateUpdatePositionRequest} from "../../models/CreateUpdatePositionRequest";
import PositionAccessRuleList from "../PositionAccessRuleList/PositionAccessRuleList";
import PositionAccessRuleEditModal from "../PositionAccessRuleEditModal/PositionAccessRuleEditModal";
import {usePositionFormContext} from "../../Contexts/PositionFormContext";
import "../../styles/card-wrapper.css";
import "./PositionForm.css";

interface Props {
    position: Position | null;
    isCreating: boolean;
    onSave: (x: CreateUpdatePositionRequest, id?: string) => void;
    onCancel: () => void;
    onClone?: () => void;
}

const dto = (value: any, id: string, type: AttributeDataType): any => {
    const x: any = {attributeDefinitionId: id};
    if (type === AttributeDataType.OneOfMany && value?.oneOfManyValueId) {
        x.oneOfManyValueId = value.oneOfManyValueId;
        return x;
    }
    if (type === AttributeDataType.String) x.stringValue = value;
    if (type === AttributeDataType.Text) x.markDownValue = value;
    if (type === AttributeDataType.Numeric) x.numericValue = value;
    if (type === AttributeDataType.Date) x.dateValue = value?.format ? value.format("YYYY-MM-DD") : value;
    if (type === AttributeDataType.Boolean) x.booleanValue = value;
    if (type === AttributeDataType.Period && Array.isArray(value)) {
        x.periodStartValue = value[0]?.format("YYYY-MM-DD");
        x.periodEndValue = value[1]?.format("YYYY-MM-DD");
    }
    return x;
};
export default function EditPositionForm({position, isCreating, onSave, onCancel, onClone}: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm();
    const [rules, setRules] = useState<AccessRule[]>([]);
    const [open, setOpen] = useState(false);
    const [current, setCurrent] = useState<AccessRule | null>(null);
    const {technologyOptions, onSearchTechnologies} = usePositionFormContext();
    useEffect(() => {
        if (position) {
            form.setFieldsValue({
                ...position,
                technologyIds: position.technologies.map(t => t.name),
                restricted: position.restricted || false,
                expertiseLevel: position.expertiseLevel ?? 0
            });
            setRules(position.accessRules || [])
        } else {
            form.resetFields();
            setRules([])
        }
    }, [position, form]);
    const saveRule = (data: { filterOperator: FilterOperator; attributeValue: AttributeValue }) => {
        setRules(x => current ? x.map(r => r.id === current.id ? {
            ...r,
            filterOperator: data.filterOperator,
            attributeValue: {...data.attributeValue, id: r.attributeValue.id}
        } : r) : [...x, {
            id: "",
            filterOperator: data.filterOperator,
            attributeValue: {...data.attributeValue, id: ""}
        }]);
        setOpen(false);
        setCurrent(null)
    };
    return <div className="d-flex justify-content-center position-form__outer">
        <div className="card-wrapper card-wrapper--80">
            <Form form={form} layout="vertical" onFinish={(v: any) => onSave({
                title: v.title,
                description: v.description,
                expertiseLevel: v.expertiseLevel ?? 0,
                accessRules: rules.map(r => ({
                    filterOperator: r.filterOperator,
                    attributeValue: dto(r.attributeValue.value, r.attributeValue.attributeDefinition.id, r.attributeValue.attributeDefinition.dataType),
                    attributeDataType: r.attributeValue.attributeDefinition.dataType,
                    attributeValueId: r.attributeValue.id || ""
                })),
                technologies: (v.technologyIds || []).map((name: string) => ({name})),
                maxProjects: v.maxProjects || 0,
                restricted: v.restricted || false,
                version: position?.version ?? 0
            }, position?.id)}><Form.Item label={t("position.title")} name="title"
                                         rules={[{required: true}]}><Input/></Form.Item><Form.Item
                label={t("position.description")}
                name="description"
                rules={[{required: true}]}><Input.TextArea
                rows={4}/></Form.Item><Form.Item label={t("position.maxProjects")} name="maxProjects"><InputNumber
                min={0}/></Form.Item><Form.Item label={t("position.technologies")} name="technologyIds"><Select
                mode="tags" showSearch
                filterOption={false}
                onSearch={onSearchTechnologies}
                options={technologyOptions}/></Form.Item><Form.Item
                label={t("position.expertiseLevel")} name="expertiseLevel"><Select
                options={[{value: 0, label: t("position.junior")}, {value: 1, label: t("position.middle")}, {
                    value: 2,
                    label: t("position.senior")
                }]}/></Form.Item><Form.Item name="restricted"
                                            valuePropName="checked"><Checkbox>{t("position.restrictedAccess")}</Checkbox></Form.Item><Divider>{t("position.accessRules")}</Divider><PositionAccessRuleList
                data={rules}
                handleOpen={r => {
                    setCurrent(r);
                    setOpen(true)
                }}
                handleAdd={() => {
                    setCurrent(null);
                    setOpen(true)
                }}
                handleDelete={ids => setRules(x => x.filter(r => !ids.map(String).includes(r.id)))}/><Divider/>
                <div className="d-flex justify-content-end gap-2"><Button
                    onClick={onCancel}>{t("app.cancel")}</Button>{!isCreating && onClone &&
                    <Button onClick={onClone}>{t("position.clone")}</Button>}<Button type="primary"
                                                                                     htmlType="submit">{isCreating ? t("app.create") : t("app.save")}</Button>
                </div>
            </Form><PositionAccessRuleEditModal
            open={open} onClose={() => {
            setOpen(false);
            setCurrent(null)
        }} onSave={saveRule} initialRule={current ? {
            filterOperator: current.filterOperator,
            attributeValue: current.attributeValue
        } : null}/></div>
    </div>
}
