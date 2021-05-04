#!/bin/bash

run_test () {
    dotnet test StateMachines.IntegrationTests/ \
    --no-restore \
    --no-build \
    --logger "$1" \
    --results-directory TestResults \
    -- "TestRunParameters.Parameter(name=\"ServiceProvider\",value=\"$2\")"
}

LOGGER_VALUE="console;verbosity=detailed"

if [ -z ${1+x} ];then
    echo "Assuming default logger \"${LOGGER_VALUE}\""
else
    LOGGER_VALUE="$1"
    echo "Logger set to \"${LOGGER_VALUE}\""
fi

TEST_VALUES=(
    "WithSqlServer"
    "WithPostgreSql"
    "WithPomeloMySqlAndDefaultLocking"
    "WithOracleMySqlAndDefaultLocking"
    "WithPomeloMySqlAndChangedLocking"
    "WithOracleMySqlAndChangedLocking"
)

# build solution
dotnet build .

# run the tests
for TEST_VALUE in "${TEST_VALUES[@]}"
do
    echo "---------------------------------------------"
    echo "- Test: ${TEST_VALUE}"
    echo "---------------------------------------------"
    run_test ${LOGGER_VALUE} ${TEST_VALUE}
done
