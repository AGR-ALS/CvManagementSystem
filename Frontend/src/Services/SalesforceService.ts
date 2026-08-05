import {apiRequest} from "./apiClient";
import {CreateSalesforceAccountRequest} from "../models/CreateSalesforceAccountRequest";

const baseUrl = "/Salesforce";

export const GetSalesforceAccountStatus = async (userId: string): Promise<boolean> => {
    return await apiRequest<boolean>(`${baseUrl}/register-status/${encodeURIComponent(userId)}`, {
        method: "GET",
    });
};

export const CreateSalesforceAccount = async (request: CreateSalesforceAccountRequest): Promise<void> => {
    await apiRequest<void>(baseUrl, {
        method: "POST",
        body: JSON.stringify(request),
    });
};
