import {apiRequest} from "./apiClient";
import {Project} from "../models/Project";
import {CreateUpdateProjectRequest} from "../models/CreateUpdateProjectRequest";

const baseUrl = "/projects";

export const GetUserProjects = async (userId: string): Promise<Project[]> => {
    return await apiRequest<Project[]>(`${baseUrl}/${encodeURIComponent(userId)}`, {
        method: "GET",
    });
};

export const GetCvProjects = async (cvId: string): Promise<Project[]> => {
    return await apiRequest<Project[]>(`${baseUrl}/cv/${encodeURIComponent(cvId)}`, {
        method: "GET",
    });
};

export const CreateProject = async (userId: string, request: CreateUpdateProjectRequest): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/${encodeURIComponent(userId)}`, {
        method: "POST",
        body: JSON.stringify(request),
    });
};

export const UpdateProject = async (userId: string, projectId: string, request: CreateUpdateProjectRequest): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/${encodeURIComponent(userId)}/${encodeURIComponent(projectId)}`, {
        method: "PUT",
        body: JSON.stringify(request),
    });
};

export const DeleteProject = async (projectId: string): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/${encodeURIComponent(projectId)}`, {
        method: "DELETE",
    });
};
