import {AttributeDefinition} from "./AttributeDefinition";

export interface AttributeValue {
    id: string;
    attributeDefinition: AttributeDefinition;
    value: boolean | number | string | { start: string, end: string } | { oneOfManyValueId: string, value: string };
}