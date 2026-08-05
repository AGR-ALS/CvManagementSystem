import {apiRequest} from "./apiClient";
import {CreateSupportTicketRequest} from "../models/CreateSupportTicketRequest";

const baseUrl = "/DropBox";

export const CreateSupportTicket = async (request: CreateSupportTicketRequest): Promise<void> => {
    await apiRequest<void>(baseUrl, {
        method: "POST",
        body: JSON.stringify(request),
    });
};
