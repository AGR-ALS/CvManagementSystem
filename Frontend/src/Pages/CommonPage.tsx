import React, {useState} from "react";
import {TableProps} from "antd";
import {Toolbar} from "../Components/Toolbar/Toolbar";
import "./CommonPage.css";

interface TableComponentProps<T> {
    data: T[];
    handleOpen: (item: T) => void;
    rowSelection?: TableProps<T>['rowSelection'];
}

interface Props<T> {
    data: T[];
    TableComponent: React.ComponentType<TableComponentProps<T>>;
    onCreate?: () => void;
    onDelete?: (ids: React.Key[]) => void;
    onUnlock?: (ids: React.Key[]) => void;
    onBlock?: (ids: React.Key[]) => void;
    onRowClick?: (item: T) => void;
}

export default function CommonPage<T extends object>({
                                                         data,
                                                         TableComponent,
                                                         onCreate,
                                                         onDelete,
                                                         onUnlock,
                                                         onBlock,
                                                         onRowClick
                                                     }: Props<T>) {
    const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
    const handleOpen = (entity: T) => {
        if (onRowClick) {
            onRowClick(entity);
        }
    }

    const rowSelection: TableProps<T>['rowSelection'] = {
        selectedRowKeys,
        onChange: (keys) => setSelectedRowKeys(keys),
    };

    const hasToolbar = onCreate || onDelete || onUnlock || onBlock;

    return (
        <div className="common-page__wrapper">
            {hasToolbar && (
                <div className="common-page__toolbar">
                    <Toolbar
                        selectedRowKeys={selectedRowKeys}
                        onCreate={onCreate}
                        onDelete={onDelete}
                        onBlock={onBlock}
                        onUnlock={onUnlock}
                    />
                </div>
            )}
            <div className="common-page__content">
                <TableComponent data={data} handleOpen={handleOpen} rowSelection={rowSelection}/>
            </div>
        </div>
    )
}