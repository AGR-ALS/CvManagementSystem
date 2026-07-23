import {apiRequest} from "./apiClient";
import {CvBasicModel} from "../models/CvBasicModel";
import {CvResponse} from "../models/CvResponse";
import {CreateUpdateCvRequest} from "../models/CreateUpdateCvRequest";

const baseUrl = "/cvs";

export const GetUserCvs = async (userId: string): Promise<CvBasicModel[]> => {
    return await apiRequest<CvBasicModel[]>(`${baseUrl}/${encodeURIComponent(userId)}`, {
        method: "GET",
    });
};

export const ResolveCv = async (userId: string, positionId: string): Promise<CvResponse> => {
    return await apiRequest<CvResponse>(`${baseUrl}/${encodeURIComponent(userId)}/${encodeURIComponent(positionId)}`, {
        method: "POST",
    });
};

export const GetCv = async (userId: string, positionId: string): Promise<CvResponse> => {
    return await apiRequest<CvResponse>(`${baseUrl}/${encodeURIComponent(userId)}/${encodeURIComponent(positionId)}`, {
        method: "GET",
    });
};

export const GetAllCvs = async (): Promise<CvBasicModel[]> => {
    const response = await apiRequest<any[]>(baseUrl, {
        method: "GET",
    });

    return response.map((item) => ({
        ...item,
        userId: item.userId ?? item.user?.id ?? "",
    })) as CvBasicModel[];
};

export const UpdateCv = async (userId: string, positionId: string, request: CreateUpdateCvRequest): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/${encodeURIComponent(userId)}/${encodeURIComponent(positionId)}`, {
        method: "PUT",
        body: JSON.stringify(request),
    });
};

export const DeleteCv = async (id: string): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/${encodeURIComponent(id)}`, {
        method: "DELETE",
    });
};

export const LikeCv = async (id: string, userId: string): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/like/${encodeURIComponent(id)}/${encodeURIComponent(userId)}`, {
        method: "POST",
    });
};

export const RemoveLike = async (id: string, userId: string): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/remove-like/${encodeURIComponent(id)}/${encodeURIComponent(userId)}`, {
        method: "POST",
    });
};

export const CheckIfUserLikedCv = async (id: string, userId: string): Promise<boolean> => {
    return await apiRequest<boolean>(`${baseUrl}/like/${encodeURIComponent(id)}/${encodeURIComponent(userId)}`, {
        method: "GET",
    });
};

export const PublishCv = async (id: string): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/publish/${encodeURIComponent(id)}`, {
        method: "GET",
    });
};

export const GetCvsAmount = async (): Promise<number> => {
    return await apiRequest<number>(`${baseUrl}/amount`, {
        method: "GET",
    });
};
