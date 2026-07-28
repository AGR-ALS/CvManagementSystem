import {AttributeCategory} from "./AttributeCategory";
import {AttributeDataType} from "./AttributeDataType";
import {OneOfManyOption} from "./OneOfManyOption";

export interface AttributeDefinition {
    id: string;
    name: string;
    dataType: AttributeDataType;
    oneOfManyOptions: OneOfManyOption[] | null;
    attributeCategoryId: string;
    attributeCategory?: AttributeCategory;
}