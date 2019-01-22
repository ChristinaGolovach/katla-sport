import { Component, OnInit } from '@angular/core';
import { HiveListItem } from '../models/hive-list-item';
import { HiveService } from '../services/hive.service';
import { ErrorHandlerService } from '../services/error-handler.service';

@Component({
  selector: 'app-hive-list',
  templateUrl: './hive-list.component.html',
  styleUrls: ['./hive-list.component.css']
})
export class HiveListComponent implements OnInit {

  hives: HiveListItem[];

  constructor(
    private hiveService: HiveService,
    private errorService : ErrorHandlerService) { }

  ngOnInit() {
    this.getHives();
  }

  getHives() {
    this.hiveService.getHives().subscribe(h => this.hives = h);
  }

  onDelete(hiveId: number) {
    this.setStatus(hiveId, true);
  }

  onUnDelete(hiveId: number) {
    this.setStatus(hiveId, false);
  }

  private setStatus(hiveId: number, isDeleted: boolean){
    var hive = this.hives.find(h => h.id == hiveId);

    //if (hive == null) ask - whether we should send a request to the server
    this.hiveService.setHiveStatus(hiveId, isDeleted)
                    .subscribe(() => hive.isDeleted = isDeleted, error => alert(this.errorService.handleError(error)));
  }
}
