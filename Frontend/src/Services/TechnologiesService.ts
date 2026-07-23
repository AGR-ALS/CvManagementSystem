import {apiRequest} from "./apiClient";
import {Technology} from "../models/Technology";

const baseUrl = "/Technologies";

export const SearchTechnologies = async (query: string): Promise<Technology[]> => {
    return await apiRequest<Technology[]>(`${baseUrl}?query=${encodeURIComponent(query)}`, {
        method: "GET",
    });
};