import {AttributeValue} from "../models/AttributeValue";
import {TableProps} from "antd";
import {createContext, useContext} from "react";

export interface AttributeContextType {
    data: AttributeValue[],
    handleOpen: (attributeValue: AttributeValue) => void;
    handleAdd: () => void;
    handleDelete: (ids: React.Key[]) => void;
    rowSelection?: TableProps<AttributeValue>['rowSelection'];
    readOnly?: boolean;
}

export const AttributeContext = createContext<AttributeContextType | undefined>(undefined);

export function useAttributeContext() {
    const context = useContext(AttributeContext);
    if (!context) {
        throw new Error('AttributeContext must be defined');
    }

    return context;
}
