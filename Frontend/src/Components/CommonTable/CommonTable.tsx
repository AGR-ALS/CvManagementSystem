import {CSSProperties} from 'react';
import {Table, TableColumnsType, TableProps} from 'antd';
import "./CommonTable.css";

interface Props<T> {
    dataSource: T[];
    columns: TableColumnsType<T>;
    rowKey: keyof T;
    onRowClick?: (record: T) => void;
    rowSelection?: TableProps<T>['rowSelection'];
    rowStyle?: (record: T) => CSSProperties | undefined;
}

export default function GenericTable<T extends object>(
    {dataSource, columns, rowKey, onRowClick, rowSelection, rowStyle}: Props<T>) {

    return (
        <div className="d-flex justify-content-center align-items-start">
            <div className="common-table__inner">
                <Table<T>
                    dataSource={dataSource}
                    columns={columns}
                    rowKey={rowKey as string}
                    tableLayout="fixed"
                    onRow={(record) => ({
                        onClick: () => onRowClick?.(record),
                        className: onRowClick ? 'common-table__row--clickable' : undefined,
                        style: rowStyle?.(record),
                    })}
                    rowSelection={rowSelection}
                    pagination={false}
                />
            </div>
        </div>
    );
}