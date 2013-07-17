CC	 := gcc
TARGET	 := pente.out
SRCS	 := board.c
OBJS     := ${SRCS:.c=.o}
	
	
all:: ${TARGET}

${TARGET} : ${OBJS}
	${CC} src/${OBJS} -o ${TARGET}

${OBJS} :
	${CC} src/${SRCS}
