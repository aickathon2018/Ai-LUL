import requests 
import json   
import glob
import os
import csv
import time
import uuid
#file = open('data.csv', 'w')
#file.write("Age,Gender,Angry,Disgust,Fear,Happy,Sad,Surprised,Neutral,Style1,VStyle1,Style2,VStyle2,Style3,VStyle3\n")
#file.close()
array_of_files_done = []
while(1):
    if os.listdir('OUTPUT_PICS_body') != []: 
        list_of_files = glob.glob('OUTPUT_PICS_body\\*.jpg') # * means all if need specific format then *.csv
        latest_file = max(list_of_files, key=os.path.getctime)
       
        print(latest_file)
        file_fashion = latest_file
        file_face = latest_file
           
        if((latest_file not in array_of_files_done) and latest_file != '') :
            row_of_data = []
            rand_num = uuid.uuid4()
            row_of_data.append(str(rand_num)[:8])
            # url name
            url_fashion = "https://fashion.recoqnitics.com/analyze"
            url_face = "https://face.recoqnitics.com/analyze"
            accessKey = "ac6b3783f2502327702e"
            secretKey = "5b13df2908be6e20af20d2f4cc96ae6f9832acef"
            data = {'access_key': accessKey,'secret_key': secretKey}# access_key and secret_key
            filename_fashion = {'filename': open(file_fashion,'rb')}
            filename_face = {'filename': open(file_face,'rb')}

            fashion = requests.post(url_fashion, files = filename_fashion, data=data)
            face = requests.post(url_face,files = filename_face,data=data)
            print(face.content)

            try:
                content_fashion = json.loads(fashion.content)
                face_detection = json.loads(face.content)
            except:
                print("JSON Error, image is not compatible!")
            try:
                for i in range(0,1,1):
                    #print("The age is: " , face_detection["faces"][i]["age"])
                    #print("The gender of the person is: " ,face_detection["faces"][i]["gender"]['value'])
                    #print("The emotion of the person is: " ,face_detection["faces"][i]["emotions"])
                    row_of_data.append(str(face_detection["faces"][i]["age"]))
                    row_of_data.append(str(face_detection["faces"][i]["gender"]['value']))
                    for key in face_detection["faces"][i]["emotions"]:
                        #emotion_string = (str(key)+ ':'+ str(face_detection["faces"][i]["emotions"][key]))
                        emotion_string = (str(face_detection["faces"][i]["emotions"][key]))
                        #row_of_data.append(key+face_detection["faces"][i]["emotions"][key])
                        row_of_data.append(emotion_string)
                    #print("\n\n")

                for i in range(0,len(content_fashion["person"]["styles"]),1):
                    #print(content_fashion["person"]["styles"][i]['styleName'],"with an accuracy of:",content_fashion["person"]["styles"][i]['confidence'])
                    #style_string = (str(content_fashion["person"]["styles"][i]['styleName']) +':'+ str(content_fashion["person"]["styles"][i]['confidence']))
                    #row_of_data.append(style_string)
                    row_of_data.append(str(content_fashion["person"]["styles"][i]['styleName']))
                    row_of_data.append(str(content_fashion["person"]["styles"][i]['confidence']))
                #print("\n\n")
                array_of_files_done.append(latest_file)
                print(row_of_data)
                file = open('data.csv', 'a+', newline='')
                writer = csv.writer(file)
                writer.writerow(row_of_data)
                file.close()
            except:
                print("API Timeout--retrying...");
            #time.sleep(5) 
           
    #else:
    #    print("NO NEW FILES!")
