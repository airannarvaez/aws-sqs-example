using System;
using System.Linq;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //the url for our queue
            var queueUrl = "https://sqs.eu-west-1.amazonaws.com/[USERID]/[QUEUENAME]";

            Console.WriteLine("Queue Test Starting!");

            Console.WriteLine("Creating Client and request");

            //Create some Credentials with our IAM user
            var awsCreds = new BasicAWSCredentials("[ACCESSKEY]", "[SECRETKEY]");

            //Create a client to talk to SQS - Verify the region of SQS
            var amazonSQSClient = new AmazonSQSClient(awsCreds,Amazon.RegionEndpoint.EUWest1);

            //Create the request to send
            var sendRequest = new SendMessageRequest(); 
            sendRequest.QueueUrl = queueUrl;
            sendRequest.MessageBody = "{ 'message' : 'hello world' }";

            //Send the message to the queue and wait for the result
            Console.WriteLine("Sending Message");
            var sendMessageResponse = amazonSQSClient.SendMessageAsync(sendRequest).Result;

            Console.WriteLine("Receiving Message");

            //Create a receive requesdt to see if there are any messages on the queue
            var receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = queueUrl;

            //Send the receive request and wait for the response
            var response = amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest).Result;

            //If we have any messages available
            if(response.Messages.Any())
            {
                foreach(var message in response.Messages)
                {
                    //Spit it out
                    Console.WriteLine(message.Body);

                    //Remove it from the queue as we don't want to see it again
                    var deleteMessageRequest = new DeleteMessageRequest();
                    deleteMessageRequest.QueueUrl = queueUrl;
                    deleteMessageRequest.ReceiptHandle = message.ReceiptHandle;

                    var result = amazonSQSClient.DeleteMessageAsync(deleteMessageRequest).Result;
                }
            }

        }
    }
}
