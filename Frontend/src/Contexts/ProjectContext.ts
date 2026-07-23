import {createContext, useContext} from "react";
import {Project} from "../models/Project";

export interface ProjectContextType {
    projects: Project[];
    handleOpenCreation: () => void;
    handleOpenProject: (project: Project) => void;
    handleCancel: () => void;
    handleDelete: (id: string) => void;
    readOnly?: boolean;
}

export const ProjectContext = createContext<ProjectContextType | undefined>(undefined);

export function useProjectContext() {
    const project = useContext(ProjectContext);

    if (!project) {
        throw new Error('ProjectContext must be defined');
    }

    return project;
}
