//
//  ViewController.m
//  Spotter2
//
//  Created by students@deti on 16/05/2017.
//  Copyright © 2017 students@deti. All rights reserved.
//

#import "ViewController.h"

@interface ViewController () {
   // __weak IBOutlet MKMapView *mapView;
    
    CLLocationManager *locationManager;
}
@property (nonatomic) BOOL isReading;



@property (nonatomic, strong) AVCaptureSession *captureSession;
@property (nonatomic, strong) AVCaptureVideoPreviewLayer *videoPreviewLayer;

-(BOOL)startReading;
-(void)stopReading;
@end

@implementation ViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    mapview.showsUserLocation=YES;
    mapview.showsBuildings=YES;

    locationManager=[CLLocationManager new];
    if ([locationManager respondsToSelector:@selector(requestWhenInUseAuthorization)]){
        [locationManager requestWhenInUseAuthorization];
    }
    [locationManager startUpdatingLocation];
    // Do any additional setup after loading the view, typically from a nib.
    
    [self loadMapPins];
    
    //QRCODE
    _isReading = NO;
    _captureSession = nil;
    
}

- (void)loadMapPins
{
    //Locations list
    NSString* path = [[NSBundle mainBundle]pathForResource:@"dataDictionary" ofType:@"plist"];
    NSArray* dataDictionary = [[NSArray alloc]initWithContentsOfFile:path];

    for (int i=0; i<dataDictionary.count; i++){
        
        NSMutableDictionary *dictionary = [NSMutableDictionary dictionaryWithDictionary:[dataDictionary objectAtIndex:i]];
        
        double latitude = [[dictionary objectForKey:@"Latitude"] doubleValue];
        double longitude = [[dictionary objectForKey:@"Longitude"] doubleValue];
        
        CLLocationCoordinate2D coord = {.latitude =
            latitude, .longitude =  longitude};
        MKCoordinateRegion region = {coord};
        
        MapPin *annotation = [[MapPin alloc] init];
        annotation.title = [dictionary objectForKey:@"Name"];
        annotation.subtitle = [dictionary objectForKey:@"City"];
        annotation.coordinate = region.center;
        [mapview addAnnotation:annotation];
    }
}


- (IBAction)startStopReading:(id)sender {
    if (!_isReading) {
        if ([self startReading]) {
            [_bbitemStart setTitle:@"Stop"];
            [_lblStatus setText:@"Scanning for QR Code..."];
        }
    }
    else{
        [self stopReading];
        [_bbitemStart setTitle:@"Start!"];
    }
    
    _isReading = !_isReading;
}
- (BOOL)startReading {
    NSError *error;
    
    AVCaptureDevice *captureDevice = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:captureDevice error:&error];
    
    if (!input) {
        NSLog(@"%@", [error localizedDescription]);
        return NO;
    }
    
    _captureSession = [[AVCaptureSession alloc] init];
    [_captureSession addInput:input];
    
    AVCaptureMetadataOutput *captureMetadataOutput = [[AVCaptureMetadataOutput alloc] init];
    [_captureSession addOutput:captureMetadataOutput];
    
    dispatch_queue_t dispatchQueue;
    dispatchQueue = dispatch_queue_create("myQueue", NULL);
    [captureMetadataOutput setMetadataObjectsDelegate:self queue:dispatchQueue];
    [captureMetadataOutput setMetadataObjectTypes:[NSArray arrayWithObject:AVMetadataObjectTypeQRCode]];
    
    _videoPreviewLayer = [[AVCaptureVideoPreviewLayer alloc] initWithSession:_captureSession];
    [_videoPreviewLayer setVideoGravity:AVLayerVideoGravityResizeAspectFill];
    [_videoPreviewLayer setFrame:_viewPreview.layer.bounds];
    [_viewPreview.layer addSublayer:_videoPreviewLayer];
    
    
    [_captureSession startRunning];
    
    return YES;
}

-(void)captureOutput:(AVCaptureOutput *)captureOutput didOutputMetadataObjects:(NSArray *)metadataObjects fromConnection:(AVCaptureConnection *)connection{
    if (metadataObjects != nil && [metadataObjects count] > 0) {
        AVMetadataMachineReadableCodeObject *metadataObj = [metadataObjects objectAtIndex:0];
        if ([[metadataObj type] isEqualToString:AVMetadataObjectTypeQRCode]) {
            [_lblStatus performSelectorOnMainThread:@selector(setText:) withObject:[metadataObj stringValue] waitUntilDone:NO];
            
            
            //Verificar se QR Code corresponde a um pin
           // NSString* path = [[NSBundle mainBundle]pathForResource:@"dataDictionary" ofType:@"plist"];
         
            
            NSArray *path = NSSearchPathForDirectoriesInDomains(NSLibraryDirectory,NSUserDomainMask,YES);
            NSString *documentsDirectory = [path objectAtIndex:0];
            
            NSString *filePath = [documentsDirectory stringByAppendingPathComponent:@"_dataDictionary.plist"];
            
            
            // If the file doesn't exist in the Documents Folder, copy it.
            NSFileManager *fileManager = [NSFileManager defaultManager];
            
            if (![fileManager fileExistsAtPath:filePath]) {
                NSString *sourcePath = [[NSBundle mainBundle] pathForResource:@"dataDictionary" ofType:@"plist"];
                [fileManager copyItemAtPath:sourcePath toPath:filePath error:nil];
            }
            
            NSMutableArray* dataDictionary = [[NSMutableArray alloc]initWithContentsOfFile:filePath];
            NSLog(@"%@", );
            
            for (int i=0; i<dataDictionary.count; i++){
                
                NSMutableDictionary *dictionary = [NSMutableDictionary dictionaryWithDictionary:[dataDictionary objectAtIndex:i]];
                NSString *title = (NSString*) [dictionary objectForKey:@"Name"];
                
                //Comparar title com QR Code label
                if([title isEqualToString:[metadataObj stringValue]]) {
                    [dictionary setValue:@YES forKey:@"PinChecked"];
                    [dictionary setValue:@"Chupaa!" forKey:@"Name"];
                    [dataDictionary replaceObjectAtIndex:i withObject:dictionary];
                }
            }
            
            for (int i=0; i<dataDictionary.count; i++){
                
                NSMutableDictionary *dictionary = [NSMutableDictionary dictionaryWithDictionary:[dataDictionary objectAtIndex:i]];
                for (id key in dictionary) {
                    NSLog(@"key: %@, value: %@ \n", key, [dictionary objectForKey:key]);
                }
            }
            
           // NSString *string = @"olaaa";
            bool success = [dataDictionary writeToFile:filePath atomically: YES];
            NSLog(success ? @"YES":@"NO");

            
            [self performSelectorOnMainThread:@selector(stopReading) withObject:nil waitUntilDone:NO];
            [_bbitemStart performSelectorOnMainThread:@selector(setTitle:) withObject:@"Start!" waitUntilDone:NO];
            _isReading = NO;
        }
    }
}
-(void)stopReading{
    [_captureSession stopRunning];
    _captureSession = nil;
    
    [_videoPreviewLayer removeFromSuperlayer];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) mapView:(MKMapView *)mapView didUpdateUserLocation:(MKUserLocation *)userLocation{

    MKMapCamera *camera=[MKMapCamera cameraLookingAtCenterCoordinate:(userLocation.coordinate) fromEyeCoordinate:(CLLocationCoordinate2DMake(userLocation.coordinate.latitude, userLocation.coordinate.longitude))  eyeAltitude:10000];
    [mapView setCamera: camera animated:YES];
}
@end
