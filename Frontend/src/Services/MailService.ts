import {apiRequest} from "./apiClient";

const baseUrl = "/Mail";

export const SendVerificationEmail = async (email: string): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/send`, {
        method: "POST",
        body: JSON.stringify({email}),
    });
};
