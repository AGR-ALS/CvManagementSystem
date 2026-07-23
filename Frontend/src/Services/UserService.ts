import {apiRequest} from "./apiClient";
import {UserProfile} from "../models/UserProfile";
import {ImageUrlResponse} from "../models/ImageUrl";
import {UserRole} from "../models/UserRole";

const baseUrl = "/Users";

export const GetCurrentUserId = async (): Promise<string> => {
    return (await apiRequest<string>(`${baseUrl}/current-user-id`, {
        method: "GET",
    })) ?? "";
};

export const GetCurrentUser = async (id: string): Promise<UserProfile> => {
    return await apiRequest<UserProfile>(`${baseUrl}/${id}`, {
        method: "GET",
    });
};

export const GetUserBasicInfo = async (id: string): Promise<UserProfile> => {
    return await apiRequest<UserProfile>(`${baseUrl}/${id}`, {
        method: "GET",
    });
};

export const UpdateUser = async (request: UserProfile): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/${request.id}`, {
        method: "PUT",
        body: JSON.stringify(request),
    });
};

export const UploadUserImage = async (id: string, file: File): Promise<string> => {
    const formData = new FormData();
    formData.append("Photo", file);

    return (await apiRequest<string>(`${baseUrl}/${id}/photo`, {
        method: "POST",
        body: formData,
    })) ?? "";
};

export const GetAllUsers = async (): Promise<UserProfile[]> => {
    return await apiRequest<UserProfile[]>(baseUrl, {
        method: "GET",
    });
};

export const DeleteUsers = async (ids: string[]): Promise<void> => {
    await apiRequest<void>(baseUrl, {
        method: "DELETE",
        body: JSON.stringify(ids),
    });
};

export const BlockUsers = async (ids: string[]): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/block`, {
        method: "PUT",
        body: JSON.stringify(ids),
    });
};

export const UnblockUsers = async (ids: string[]): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/unblock`, {
        method: "PUT",
        body: JSON.stringify(ids),
    });
};

export const GetUserImage = async (fileKey: string): Promise<ImageUrlResponse> => {
    return await apiRequest<ImageUrlResponse>(`${baseUrl}/photo/${encodeURIComponent(fileKey)}`, {
        method: "GET",
    });
};

export const GetCurrentUserRole = async (): Promise<UserRole> => {
    return await apiRequest<UserRole>(`${baseUrl}/current-user-role`, {
        method: "GET",
    });
};

export const GetRoles = async (): Promise<{ id: string; name: string }[]> => {
    return await apiRequest<{ id: string; name: string }[]>(`${baseUrl}/roles`, {
        method: "GET",
    });
};

export const GetCandidatesAmount = async (): Promise<number> => {
    return await apiRequest<number>(`${baseUrl}/amount/candidates`, {
        method: "GET",
    });
};

export const GetRecruitersAmount = async (): Promise<number> => {
    return await apiRequest<number>(`${baseUrl}/amount/recruiters`, {
        method: "GET",
    });
};