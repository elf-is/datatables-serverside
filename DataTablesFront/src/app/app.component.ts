import {HttpClient} from '@angular/common/http';
import {Component, OnInit} from '@angular/core';
import {Customer} from './Customer';

class DataTablesResponse {
  data: any[];
  draw: number;
  recordsFiltered: number;
  recordsTotal: number;
}


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  dtOptions: DataTables.Settings = {};
  customers: Customer[] = [];

  constructor(private http: HttpClient) {
  }

  ngOnInit(): void {
    const that = this;

    this.dtOptions = {
      pagingType: 'full_numbers',
      pageLength: 10,
      serverSide: true,
      processing: true,
      ajax: (dataTablesParameters: any, callback) => {
        that.http
          .post<DataTablesResponse>(
            'https://localhost:7284/api/DataTable',
            dataTablesParameters
          ).subscribe(resp => {
          that.customers = resp.data;

          callback({
            recordsTotal: resp.recordsTotal,
            recordsFiltered: resp.recordsFiltered,
            data: [],
            draw: resp.draw
          });
        });
      },
      columns: [
        {data: 'customer_number'},
        {data: 'customer_name'},
        {data: 'contact_first_name'},
        {data: 'contact_last_name'},
        {data: 'phone'},
        {data: 'city'},
        {data: 'state'},
        {data: 'country'},
        {data: 'credit_limit'}
      ]
    };
  }

}
