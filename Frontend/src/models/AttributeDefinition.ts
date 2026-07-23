import {AttributeCategory} from "./AttributeCategory";
import {AttributeDataType} from "./AttributeDataType";

export interface OneOfManyOption {
    id: string;
    value: string;
    oneOfManyId: string;
}

export interface AttributeDefinition {
    id: string;
    name: string;
    dataType: AttributeDataType;
    oneOfManyOptions: OneOfManyOption[] | null;
    attributeCategoryId: string;
    attributeCategory?: AttributeCategory;
}

export interface AttributeDefinitionDeleteRequest {
    ids: string[];
}