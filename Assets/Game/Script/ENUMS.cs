using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COLOR
{
    NONE = -1,
    BLUE = 0,
    RED = 1,
    YELLOW = 2,
    GREEN = 3,
    PINK = 4,
    ORANGE = 5,
    CYAN = 6,
    MAGENTA = 7,
    LIGHTGREEN = 8,
    DARKPURPLE = 9,
}

public enum LAND_STATE
{
    NONE = -1,
    FALLING = 0,
    FREE_BASKET = 1,
    COLOR_BASKET = 2,
    SLEEPY = 3,
}

public enum TYPE
{
    NONE = -1,
    BIRD = 0,
    EGG = 1,
    HOUSE = 2,
    CAGE = 3,
}

public enum STATE
{
    ACTIVE = 0,
    INACTIVE = 1,
    LANDED = 2,
    FINISH = 3,
}

public enum DIRECTION
{
    RIGHT = 0,
    LEFT = 1,
}

public enum RESOURCES
{
    UNDO = 0,
    ADDSLOT = 1,
    SHUFFLE = 2,
    MAGNET = 3,
    GOLD = 4,
    HEALTH = 5,
}

public enum FEATURE
{
    NONE = 0,
    SLEEP_BIRD = 1,
    BOOSTER_UNDO = 2,
    EGG = 3,
    HOUSE = 4,
    BOOSTER_EXTRA_SPACE = 5,
    CAGE = 6,
    BOOSTER_SHUFFLE = 7,
    BOOSTER_MAGNET = 8,
}

public enum FEATURE_ACTION
{
    NONE = 0,
    CLAIM = 1,
    CONTINUE = 2,
}
