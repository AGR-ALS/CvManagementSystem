import {Button, Form, Input, Modal, Select} from "antd";
import {useEffect} from "react";
import {useTranslation} from "react-i18next";
import {Project} from "../../models/Project";
import "./ProjectEditModal.css";

interface Props {
    open: boolean;
    project: Project | null;
    onCancel: () => void;
    onSave: (values: Project) => void;
    onDelete?: (id: string) => void;
    isCreating: boolean;
    technologyOptions: { value: string; label: string }[];
    onSearchTechnologies: (query: string) => Promise<void>;
}

interface ProjectFormValues {
    id: string;
    name: string;
    description: string;
    technologies: string[];
}

export default function ProjectEditModal({
                                             open,
                                             project,
                                             onCancel,
                                             onSave,
                                             onDelete,
                                             isCreating,
                                             technologyOptions,
                                             onSearchTechnologies
                                         }: Props) {
    const {t} = useTranslation();
    const [form] = Form.useForm<ProjectFormValues>();

    useEffect(() => {
        if (open) {
            if (isCreating) {
                form.resetFields();
            } else if (project) {
                form.setFieldsValue({
                    id: project.id,
                    name: project.name,
                    description: project.description,
                    technologies: project.technologies.map(t => t.name)
                });
            }
        }
    }, [open, project, isCreating]);

    const handleOk = async () => {
        const values = await form.validateFields();

        const projectValues: Project = {
            ...values,
            technologies: values.technologies.map(name => ({
                name
            })),
            version: project?.version ?? 0,
        };

        if (isCreating) {
            onSave({...projectValues});
        } else if (project) {
            onSave({...projectValues, id: project.id});
        }
    };

    const handleDelete = () => {
        if (!project?.id || !onDelete) {
            return;
        }
        onDelete(project.id);
        onCancel();
    };

    return (
        <Modal
            title={isCreating ? t("project.create") : t("project.edit")}
            open={open}
            onCancel={onCancel}
            footer={
                <div className="project-edit-modal__footer">
                    <div>
                        {!isCreating && onDelete && (
                            <Button danger onClick={handleDelete}>
                                {t("app.delete")}
                            </Button>
                        )}
                    </div>
                    <div className="project-edit-modal__actions">
                        <Button onClick={onCancel}>
                            {t("app.cancel")}
                        </Button>
                        <Button type="primary" onClick={handleOk}>
                            {isCreating ? t("project.createBtn") : t("project.editBtn")}
                        </Button>
                    </div>
                </div>
            }
        >
            <Form form={form} layout="vertical">
                <Form.Item
                    label={t("project.name")}
                    name="name"
                    rules={[{required: true, message: t("project.nameRequired")}]}
                >
                    <Input/>
                </Form.Item>
                <Form.Item
                    label={t("project.description")}
                    name="description"
                    rules={[{required: true, message: t("project.descRequired")}]}
                >
                    <Input/>
                </Form.Item>
                <Form.Item
                    label={t("project.technologies")}
                    name="technologies"
                >
                    <Select
                        mode="tags"
                        showSearch
                        placeholder={t("project.startTyping")}
                        filterOption={false}
                        onSearch={onSearchTechnologies}
                        options={technologyOptions}
                    />
                </Form.Item>
            </Form>
        </Modal>
    );
}
