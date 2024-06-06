import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SidebarRoomsComponent } from './sidebar-rooms.component';

describe('SidebarRoomsComponent', () => {
  let component: SidebarRoomsComponent;
  let fixture: ComponentFixture<SidebarRoomsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SidebarRoomsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SidebarRoomsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
