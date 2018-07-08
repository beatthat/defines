## Install

From your unity project folder:

    npm init
    npm install beatthat/defines --save

## Development

You can edit the code and samples in the test environment and then use ```npm run overwrite:test2src``` to sync changes back to the package src.

```
    npm run install:test
    cd test

    # edit code under Assets/Plugins/packages/beatthat/defines
    # edit samples under Assets/Samples/packages/beatthat/defines

    # sync changes back to src
    npm run overwrite:test2src
```

**REMEMBER:** changes made under the test folder are not saved to the package
unless they are copied back into the source folder
