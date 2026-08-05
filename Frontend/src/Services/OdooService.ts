import {apiRequest} from "./apiClient";

const baseUrl = "/Odoo";

export const GetPositionApiToken = async (positionId: string): Promise<string> => {
    const response = await apiRequest<{token: string}>(`${baseUrl}/position/${encodeURIComponent(positionId)}/token`, {
        method: "GET",
    });
    return response.token;
};
