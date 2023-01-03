#include <stdlib.h>
#include <stdio.h>
#include <time.h>
#include <unistd.h>
#include <math.h>
#include <ncurses.h>
#include <string.h>

#define MAX_LENGTH_SNAKE 150
#define CHAR_SNAKE 'O'
#define CHAR_WALL_TOP 'v'
#define CHAR_WALL_RIGHT '<'
#define CHAR_WALL_LEFT '>'
#define CHAR_WALL_BOTTOM '^'

#define UP 1
#define DOWN 2
#define RIGHT 3
#define LEFT 4

int rows, cols, direction, halfPerimeter;
unsigned int length_snake = 3; int points; 

// tells if game won or lost
bool won = false;
unsigned int usecs = 100000;

//structure defined with variable x and y 
struct Snake 
{
    int x;
    int y;

} snake[MAX_LENGTH_SNAKE], snakeHead;

//structure declaration of trophy 
struct Trophy
{
    int x;
    int y;
    int time;
    int clock;
    int pointValue;

} trophy;

void setDirection(int c) //function declaration of direction
{
    switch (c) //switch case definition by using four sides
    {
    case KEY_UP:
        direction = UP;
        break;

    case KEY_DOWN:
        direction = DOWN;
        break;

    case KEY_RIGHT:
        direction = RIGHT;
        break;

    case KEY_LEFT:
        direction = LEFT;
        break;
    }
}

void startGame() //startGame function declaration 
{
    srand(time(NULL));

    for (int i = 0; i < MAX_LENGTH_SNAKE; i++)
    {
        snake[i].x = -1;
        snake[i].y = -1;
        
    }

    initscr();
    keypad(stdscr, TRUE);

    getmaxyx(stdscr, rows, cols);
    halfPerimeter = (rows + cols);

    noecho();
    cbreak();

    clear();
}


void drawSnakePit() //declaration of void function 
{
    
    for (int i = 0; i < rows; i++)
    {
        mvaddch(i, 0, CHAR_WALL_LEFT);              //draws the LEFT VERTICAL wall
        mvaddch(i, cols - 1, CHAR_WALL_RIGHT);      //draws the RIGHT VERTICAL wall
    }

    for (int i = 0; i < (cols) ; i++)
    {
        mvaddch(0, i, CHAR_WALL_TOP);               //draws the TOP HORIZONTAL wall
        mvaddch(rows - 1, i, CHAR_WALL_BOTTOM);     //draws the BOTTOM HORIZONTAL wall
    }       
            mvprintw(0, cols / 5, " Warning: you only live once!  Current score: %d ",points);
}


void addTrophy()
{
    int x, y, pointValue, time;

    x = rand() % (cols - 4) + 1;            //controls trophy position, subtraction must be
    y = rand() % (rows - 4) + 1;             //4 or more so trophy doesn't show out of window

    pointValue = (rand() % 9) + 1;          //controls worth of trophy, 1 to 10 points
    
					// controls time before trophy disappears based off
					// distance from snake and length of snake + random.
    if ((abs(trophy.x - snake[0].x) + abs(trophy.y - snake[0].y)) >= 30)
        trophy.time = ((rand() % 18000) + (length_snake*500)+8000);
    else if ((abs(trophy.x - snake[0].x) + abs(trophy.y - snake[0].y)) >= 15)
        trophy.time = ((rand() % 19000) + (length_snake*400)+7000);
    else
        trophy.time = ((rand() % 20000) + (length_snake*300)+6000);
    

   
    trophy.x = x;
    trophy.y = y;

    trophy.pointValue = pointValue;
    trophy.clock = clock();
    
}


void drawTrophy()
{
    move(trophy.y, trophy.x);

    switch (trophy.pointValue) //switch cases for determining size of adding when the snake eats the trophy, its length is increased by the corresponding number of characters.

    {
    case 1:
        addch('1');
        break;

    case 2:
        addch('2');
        break;

    case 3:
        addch('3');
        break;

    case 4:
        addch('4');
        break;

    case 5:
        addch('5');
        break;

    case 6:
        addch('6');
        break;

    case 7:
        addch('7');
        break;

    case 8:
        addch('8');
        break;

    case 9:
        addch('9');
        break;
    }
}

void startSnake(int length_snake) //Use for starting the snake at random location inside the pit 
{
    int random_x = rand() % (cols / 2) + cols/4;
    int random_y = rand() % (rows / 2) + rows/4;

    direction = (rand() % 3) + 1; //initial direction is a number 1,2,3,4 (UP,DOWN,RIGHT,LEFT), 

    for (int i = 0; i < length_snake - 1; i++)
    {
        snake[i].x = random_x - i;
        snake[i].y = random_y;
    }
}

void drawSnake()  //function to draw snake with character that defined
{
    int i = 0;

    while (snake[i].x != -1 && snake[i].y != -1)
    {
        move(snake[i].y, snake[i].x);
        addch(CHAR_SNAKE);
        i++;
    }
}

void shiftSnakeCells()
{
    for (int i = length_snake - 1; i > 0; i--)
    {
        snake[i] = snake[i - 1];
    }
}

void addSnakeHead() // Use to adding snake header
{
    snakeHead = snake[0];

    shiftSnakeCells();

    switch (direction)
    {
    case UP:
        snakeHead.y--;
        break;

    case DOWN:
        snakeHead.y++;
        break;

    case RIGHT:
        snakeHead.x++;
        break;

    case LEFT:
        snakeHead.x--;
        break;
    }

    snake[0] = snakeHead;
}

void moveSnake() // Use to define snake movement
{
    addSnakeHead();

    mvaddch(snake[0].y, snake[0].x, CHAR_SNAKE);
    
    if (snake[0].x == trophy.x && snake[0].y == trophy.y)  //if the snake position overlaps the trophy
    {
        length_snake += trophy.pointValue;
        points = points + trophy.pointValue;
        addTrophy();
        usecs = usecs -((length_snake/2) * 300);                              //game speeds up every time trophy is obtained
    }
}


bool checkGameRules(int c)
{
    if (length_snake > halfPerimeter)
    {
        won = true;
        return false;
    }

     if (snake[0].y <= 0 || snake[0].x <= 0 || snake[0].y >= rows - 1 || snake[0].x >= cols - 1)
        return false;

    switch (c)
    {
    case UP:
        if (direction == DOWN)
        {
            timeout(-1);
            getch();

            return false;
        }
        break;

    case DOWN:
        if (direction == UP)
        {
            timeout(-1);
            getch();

            return false;
        }

        break;

    case RIGHT:
        if (direction == LEFT)
        {
            timeout(-1);
            getch();

            return false;
        }

        break;

    case LEFT:
        if (direction == RIGHT)
        {
            timeout(-1);
            getch();

            return false;
        }

        break;

    default:
        break;
    }

    // checking if it ran into itself
    for (int i = 1; i < length_snake; ++i)
    {
        if (snake[0].x == snake[i].x && snake[0].y == snake[i].y)
        {
            return false;
        }
    }

    return true;
}

int main(int argc, char *argv[] ) //main body of program to call the function and game rule
{

    int c; c = 3; //needs to be initialized or loop breaks on game start

    clock_t old = clock(), diff;
    
    while (c !=5)
    {

    getch();
    
    startGame();
    
    startSnake(length_snake);
    drawSnake();
    drawSnakePit();
    drawTrophy();
    addTrophy();
    curs_set(0);
    nodelay(stdscr,TRUE);
    
    while (checkGameRules(c)) ////while statement to check game rule
    {
        c = getch();
        
        setDirection(c);
        // checkGameRules(c);

        clear();
        drawTrophy();
        drawSnakePit();
        moveSnake();
        drawSnake();
        
        clock_t current = clock();
        if (current-trophy.clock >= trophy.time){
            addTrophy();
        }
        
        refresh();
        
    
            usleep(usecs);
       
        
      
    }

    clear();

   
    refresh();

    getch();
    endwin();

    clear();

    if (won)//Use if to check game condition 
    {
        mvprintw(rows / 2, cols / 2 - 9, "You won, double press a button to play again!");
              mvprintw(rows / 4, cols / 2 - 9,"\n\n Final score: %d \n\n", points);

        usecs = 100000;
        length_snake = 3;
        points = 0;
    }
    else
    {
        mvprintw(rows / 2, cols / 2 - 9, "You lost, double press a button to  play again.. ");
              mvprintw(rows / 4, cols / 2 - 9,"\n\n Final score: %d \n\n", points);

        usecs = 100000;
        length_snake = 3;
        points = 0;

    }
        nodelay(stdscr,FALSE); // this screen is set to delay mode so game doesn’t start automatically

        getch();
        
    refresh();

    endwin();

    clear();
    }
}









