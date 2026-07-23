export interface CreateUpdateAttributeValueRequest {
    attributeDefinitionId: string;
    stringValue?: string | null;
    markDownValue?: string | null;
    imageValue?: File | null;
    numericValue?: number | null;
    dateValue?: string | null;
    periodStartValue?: string | null;
    periodEndValue?: string | null;
    booleanValue?: boolean | null;
    oneOfManyValueId?: string | null;
}