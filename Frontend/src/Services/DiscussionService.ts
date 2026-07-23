import {apiRequest} from "./apiClient";
import {Discussion} from "../models/Discussion";
import {CreateUpdateDiscussionMessageRequest} from "../models/DiscussionDto";

const baseUrl = "/discussion";

export const GetDiscussion = async (positionId: string): Promise<Discussion> => {
    return await apiRequest<Discussion>(`${baseUrl}/${positionId}`, {
        method: "GET",
    });
};

export const SendMessage = async (request: CreateUpdateDiscussionMessageRequest): Promise<void> => {
    await apiRequest<void>(baseUrl, {
        method: "POST",
        body: JSON.stringify(request),
    });
};