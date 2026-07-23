import {apiRequest} from "./apiClient";
import {AttributeCategory} from "../models/AttributeCategory";
import {AttributeDefinition} from "../models/AttributeDefinition";
import {AttributeValue} from "../models/AttributeValue";
import {ImageUrlResponse} from "../models/ImageUrl";
import {CreateUpdateAttributeValueRequest} from "../models/CreateUpdateAttributeValueRequest";
import {AttributeDataType} from "../models/AttributeDataType";

const baseUrl = "/attributes";

export const GetUserAttributes = async (userId: string): Promise<AttributeValue[]> => {
    return await apiRequest<AttributeValue[]>(`${baseUrl}/${encodeURIComponent(userId)}`, {
        method: "GET",
    });
};

export const GetAttributeDefinitions = async (): Promise<AttributeDefinition[]> => {
    return await apiRequest<AttributeDefinition[]>(baseUrl, {
        method: "GET",
    });
};

export const GetAttributeCategories = async (): Promise<AttributeCategory[]> => {
    return await apiRequest<AttributeCategory[]>(`${baseUrl}/categories`, {
        method: "GET",
    });
};

export const CreateAttributeDefinitions = async (attributeDefinition: AttributeDefinition): Promise<void> => {
    const payload = {
        name: attributeDefinition.name,
        attributeCategoryId: attributeDefinition.attributeCategoryId,
        dataType: Number(attributeDefinition.dataType),
        oneOfManyOptions: attributeDefinition.oneOfManyOptions ?? null,
    };

    await apiRequest<void>(baseUrl, {
        method: "POST",
        body: JSON.stringify(payload),
    });
};

export const EditAttributeDefinitions = async (attributeDefinition: AttributeDefinition): Promise<void> => {
    const payload = {
        name: attributeDefinition.name,
        attributeCategoryId: attributeDefinition.attributeCategoryId,
        dataType: Number(attributeDefinition.dataType),
        oneOfManyOptions: attributeDefinition.oneOfManyOptions ?? null,
    };

    await apiRequest<void>(`${baseUrl}/${attributeDefinition.id}`, {
        method: "PUT",
        body: JSON.stringify(payload),
    });
};

export const DeleteAttributeDefinitions = async (ids: string[]): Promise<void> => {
    await apiRequest<void>(baseUrl, {
        method: "DELETE",
        body: JSON.stringify(ids),
    });
};

export const GetAttributeImage = async (fileKey: string): Promise<ImageUrlResponse> => {
    return await apiRequest<ImageUrlResponse>(`${baseUrl}/photo/${encodeURIComponent(fileKey)}`, {
        method: "GET",
    });
};

export const DeleteAttributeValues = async (ids: string[]): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/user`, {
        method: "DELETE",
        body: JSON.stringify(ids),
    });
};

export const AddAttributeValueToUser = async (
    userId: string,
    type: AttributeDataType,
    request: CreateUpdateAttributeValueRequest
): Promise<void> => {
    const formData = makeFormData(request);

    await apiRequest<void>(`${baseUrl}/user/${userId}?attributeType=${type}`, {
        method: "POST",
        body: formData,
    });
};

function makeFormData(request: CreateUpdateAttributeValueRequest) {
    const formData = new FormData();
    formData.append("AttributeDefinitionId", request.attributeDefinitionId);
    if (request.stringValue != null) formData.append("StringValue", request.stringValue);
    if (request.markDownValue != null) formData.append("MarkDownValue", request.markDownValue);
    if (request.imageValue != null) formData.append("ImageValue", request.imageValue);
    if (request.numericValue != null) formData.append("NumericValue", String(request.numericValue));
    if (request.dateValue != null) formData.append("DateValue", request.dateValue);
    if (request.periodStartValue != null) formData.append("PeriodStartValue", request.periodStartValue);
    if (request.periodEndValue != null) formData.append("PeriodEndValue", request.periodEndValue);
    if (request.booleanValue != null) formData.append("BooleanValue", String(request.booleanValue));
    if (request.oneOfManyValueId != null) formData.append("OneOfManyValueId", request.oneOfManyValueId);
    return formData;
}

export const UpdateAttributeValueToUser = async (
    id: string,
    type: AttributeDataType,
    request: CreateUpdateAttributeValueRequest
): Promise<void> => {
    const formData = makeFormData(request);

    await apiRequest<void>(`${baseUrl}/user/${id}?attributeType=${type}`, {
        method: "PUT",
        body: formData,
    });
};
