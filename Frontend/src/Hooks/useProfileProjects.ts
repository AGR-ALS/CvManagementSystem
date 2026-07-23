import {useState} from "react";
import {message} from "antd";
import {useTranslation} from "react-i18next";
import {Project} from "../models/Project";
import {CreateUpdateProjectRequest} from "../models/CreateUpdateProjectRequest";
import {CreateProject, DeleteProject, GetUserProjects, UpdateProject} from "../Services/ProjectService";

export function useProfileProjects(userId: string, onProjectsChange: (projects: Project[]) => void) {
    const {t} = useTranslation();
    const [isProjectModalOpen, setIsProjectModalOpen] = useState(false);
    const [isProjectModalCreating, setIsProjectModalCreating] = useState(false);
    const [currentProject, setCurrentProject] = useState<Project | null>(null);
    const refresh = async () => onProjectsChange(await GetUserProjects(userId));
    const onSave = async (project: Project) => {
        try {
            const request: CreateUpdateProjectRequest = {
                name: project.name,
                description: project.description,
                technologies: project.technologies,
                version: project.version
            };
            if (isProjectModalCreating) await CreateProject(userId, request);
            else await UpdateProject(userId, project.id, request);
            await refresh();
            message.success(isProjectModalCreating ? t("messages.projectCreated") : t("messages.projectUpdated"));
            setIsProjectModalOpen(false);
        } catch {
            message.error(isProjectModalCreating ? t("messages.projectCreateError") : t("messages.projectUpdateError"));
        }
    };
    const onDelete = async (id: string) => {
        try {
            await DeleteProject(id);
            await refresh();
            message.success(t("messages.projectDeleted"));
        } catch {
            message.error(t("messages.projectDeleteError"));
        }
    };
    return {
        isProjectModalOpen, isProjectModalCreating, currentProject, onSave, onDelete,
        onOpenCreation: () => {
            setIsProjectModalCreating(true);
            setCurrentProject(null);
            setIsProjectModalOpen(true);
        },
        onOpenProject: (project: Project) => {
            setIsProjectModalCreating(false);
            setCurrentProject(project);
            setIsProjectModalOpen(true);
        },
        onCancel: () => {
            setIsProjectModalOpen(false);
            setCurrentProject(null);
        },
    };
}
