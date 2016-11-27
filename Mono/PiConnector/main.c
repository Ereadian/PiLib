﻿#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h> 
#include <sys/socket.h>
#include <netinet/in.h>
#include <wiringPi.h>

#define LedPin 0

#define COMMAND_END 0
#define COMMAND_PIN_SET_MODE 1
#define COMMAND_PIN_SET_VALUE 2
#define COMMAND_PIN_GET_VALUE 3

int main (int argc, char *argv[])
{
	int sockfd, newsockfd, portno, n, run;
	socklen_t clilen;
	struct sockaddr_in serv_addr, cli_addr;
	char buffer[10];

	portno = argc < 2 ? 5555 : atoi(argv[1]);

	if (wiringPiSetup() == -1)
	{
		perror("Setup failed\n");
		return 1;
	}

	sockfd = socket(AF_INET, SOCK_STREAM, 0);
	if (sockfd < 0)
	{
		perror("ERROR opening socket");
		return -1;
	}

	bzero((char *) &serv_addr, sizeof(serv_addr));
	serv_addr.sin_family = AF_INET;
	serv_addr.sin_addr.s_addr = INADDR_ANY;
	serv_addr.sin_port = htons(portno);
	if (bind(sockfd, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0) 
	{
		perror("ERROR on binding");
		return -1;
	}

	listen(sockfd, 5);
	while(1)
	{
		printf("Waiting for connection\n");
		clilen = sizeof(cli_addr);
		newsockfd = accept(sockfd,  (struct sockaddr *) &cli_addr, &clilen);
		if (newsockfd < 0) 
		{
			perror("ERROR on accept");
			continue;
        }

		printf("Connected\n");
		run = 1;
		while(run)
		{
	        n = read(newsockfd, buffer, 1);
	        if (n < 0)
	        {
				perror("ERROR on read command type");
				break;
	        }

	       	switch (buffer[0])
	       	{
	       		case COMMAND_END:
		        	printf("Disconnecting...");
		        	run = 0;
		        	break;
				case COMMAND_PIN_SET_MODE:
					n = read(newsockfd, buffer, 2);
					if (n < 0)
					{
						perror("ERROR on read data for setting pin mode");
						run = 0;
					}
					else
					{
						pinMode((int)buffer[0], buffer[1] ? OUTPUT : INPUT);
					}
					break;
				case COMMAND_PIN_SET_VALUE:
					n = read(newsockfd, buffer, 2);
					if (n < 0)
					{
						perror("ERROR on read data for writting pin value");
						run = 0;
					}
					else
					{
						digitalWrite((int)buffer[0], buffer[1] ? HIGH :LOW);
					}
					break;
				case COMMAND_PIN_GET_VALUE:
					n = read(newsockfd, buffer, 1);
					if (n < 0)
					{
						perror("ERROR on read data for reading pin value");
						run = 0;
					}
					else
					{
						buffer[0] = digitalRead((int)buffer[0]);
						n = write(newsockfd, buffer, 1);
						if (n < 0)
						{
							perror("ERROR on read data for returning data after reading pin value");
							run = 0;
						}
					}
					break;
				default:
					perror("ERROR bad command id");
					run = 0;
					break;
	        }
		}
		close(newsockfd);        	
	}

	return 0;
}
