import {apiRequest} from "./apiClient";
import {Position} from "../models/Position";
import {CreateUpdatePositionRequest} from "../models/PositionDto";

const baseUrl = "/positions";

export const GetPositions = async (): Promise<Position[]> => {
    return await apiRequest<Position[]>(baseUrl, {
        method: "GET",
    });
};

export const GetPosition = async (id: string): Promise<Position> => {
    return await apiRequest<Position>(`${baseUrl}/${id}`, {
        method: "GET",
    });
};

export const GetPopularPositions = async (amount: number): Promise<Position[]> => {
    return await apiRequest<Position[]>(`${baseUrl}/popular?amount=${amount}`, {
        method: "GET",
    });
};

export const GetRecentPositions = async (amount: number): Promise<Position[]> => {
    return await apiRequest<Position[]>(`${baseUrl}/recent?amount=${amount}`, {
        method: "GET",
    });
};

export const GetPositionsAmount = async (): Promise<number> => {
    return await apiRequest<number>(`${baseUrl}/amount`, {
        method: "GET",
    });
};

export const CreatePosition = async (request: CreateUpdatePositionRequest): Promise<void> => {
    await apiRequest<void>(baseUrl, {
        method: "POST",
        body: JSON.stringify(request),
    });
};

export const UpdatePosition = async (id: string, request: CreateUpdatePositionRequest): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/${id}`, {
        method: "PUT",
        body: JSON.stringify(request),
    });
};

export const DeletePositions = async (ids: string[]): Promise<void> => {
    await apiRequest<void>(baseUrl, {
        method: "DELETE",
        body: JSON.stringify(ids),
    });
};