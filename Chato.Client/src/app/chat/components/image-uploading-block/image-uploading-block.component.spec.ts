import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageUploadingBlockComponent } from './image-uploading-block.component';

describe('ImageUploadingBlockComponent', () => {
  let component: ImageUploadingBlockComponent;
  let fixture: ComponentFixture<ImageUploadingBlockComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImageUploadingBlockComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ImageUploadingBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
