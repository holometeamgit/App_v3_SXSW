#import "UnityAppController.h"
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import <Contacts/Contacts.h>
#import <ContactsUI/ContactsUI.h>

extern UIViewController *UnityGetGLViewController();

@interface iOSContactsListPlugin : NSObject

@end

@implementation iOSContactsListPlugin

+(NSString *)getAllContacts
{
    NSString *documentsDirectory = [NSHomeDirectory() stringByAppendingPathComponent:@"Documents"];
    NSString *fileName = [documentsDirectory stringByAppendingPathComponent:@"contact_list.json"];
    [[NSFileManager defaultManager] createFileAtPath:fileName contents:[NSData data] attributes:nil];
    
    __block NSFileHandle *fileHandle = [NSFileHandle fileHandleForWritingAtPath:fileName];
    
    __block NSString *content = @"{\"ContactList\": [";
    
    if (fileHandle){
        [fileHandle seekToEndOfFile];
        [fileHandle writeData:[content dataUsingEncoding:NSUTF8StringEncoding]];
        [fileHandle closeFile];
    }
    else{
        [content writeToFile:fileName
                  atomically:NO
                    encoding:NSUTF8StringEncoding
                       error:nil];
    }
    
        CNContactStore *store = [[CNContactStore alloc] init];
            [store requestAccessForEntityType:CNEntityTypeContacts completionHandler:^(BOOL granted, NSError * _Nullable error) {
                if (granted == YES) {
                    //keys with fetching properties
                    NSArray *keys = @[CNContactFamilyNameKey, CNContactGivenNameKey, CNContactPhoneNumbersKey, CNContactImageDataKey];
                    CNContactFetchRequest *request = [[CNContactFetchRequest alloc] initWithKeysToFetch:keys];
                    NSError *error;
                    BOOL success = [store enumerateContactsWithFetchRequest:request error:&error usingBlock:^(CNContact * __nonnull contact, BOOL * __nonnull stop) {
                        if (error) {
                            NSLog(@"error fetching contacts %@", error);
                        } else {
                            NSString *GivenNameLabel = @"{\"GivenName\":\"";
                            content = GivenNameLabel;
                            content = [content stringByAppendingString:contact.givenName];
                            
                            NSString *FamilyNameLabel = @"\",\"FamilyName\":\"";
                            content = [content stringByAppendingString:FamilyNameLabel];
                            content = [content stringByAppendingString:contact.familyName];
                            
                            NSString *PhoneNumbersLabel = @"\",\"PhoneNumbers\":[";
                            content = [content stringByAppendingString:PhoneNumbersLabel];
                            
                            for (int i = 0; i < contact.phoneNumbers.count; ++i) {
                                NSString *Label = @"{\"Label\":\"";
                                content = [content stringByAppendingString:Label];
                                if (contact.phoneNumbers[i].label == nil || [contact.phoneNumbers[i].label isKindOfClass:[NSNull class]]) {
                                // TODO - maybe add fake number (?)
                                } else {
                                    content = [content stringByAppendingString:contact.phoneNumbers[i].label];
                                }
                                
                                NSString *Value = @"\",\"Value\":\"";
                                content = [content stringByAppendingString:Value];
                                
                                if (contact.phoneNumbers[i].value == nil || [contact.phoneNumbers[i].value isKindOfClass:[NSNull class]]) {
                                    // TODO - maybe add fake number (?)
                                } else {
                                    content = [content stringByAppendingString:contact.phoneNumbers[i].value.stringValue];
                                }
                                
                                NSString *End = @"\"},";
                                content = [content stringByAppendingString:End];
                            }
                            
                            if (contact.phoneNumbers.count > 0) {
                                content = [content substringToIndex:[content length] - 1];
                            }
                            
                            NSString *End = @"]},";
                            content = [content stringByAppendingString:End];
                            
                            fileHandle = [NSFileHandle fileHandleForWritingAtPath:fileName];
                            
                            if (fileHandle){
                                [fileHandle seekToEndOfFile];
                                [fileHandle writeData:[content dataUsingEncoding:NSUTF8StringEncoding]];
                                [fileHandle closeFile];
                            }
                            else{
                                [content writeToFile:fileName
                                          atomically:NO
                                            encoding:NSUTF8StringEncoding
                                               error:nil];
                            }
                        }
                    }];
                }        
            }];
    
            content = [content substringToIndex:[content length] - 1];
            NSString *End = @"]}";
            content = [content stringByAppendingString:End];
            
            fileHandle = [NSFileHandle fileHandleForWritingAtPath:fileName];
            if (fileHandle){
                [fileHandle seekToEndOfFile];
                [fileHandle writeData:[content dataUsingEncoding:NSUTF8StringEncoding]];
                [fileHandle closeFile];
            }
            else{
                [content writeToFile:fileName
                          atomically:NO
                            encoding:NSUTF8StringEncoding
                               error:nil];
            }
            
            return fileName;
}
@end

char* cStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

extern "C"
{
    const char * _GetAllContacts()
    {
        return cStringCopy([[iOSContactsListPlugin getAllContacts] UTF8String]);
    }
}
